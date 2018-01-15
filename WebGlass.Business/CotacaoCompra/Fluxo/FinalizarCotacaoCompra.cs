using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass;
using GDA;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class FinalizarCotacaoCompra : BaseFluxo<FinalizarCotacaoCompra>
    {
        private FinalizarCotacaoCompra() { }

        private class DadosFornecedor
        {
            public string Nome { get; set; }
            public uint? PlanoConta { get; set; }
            public uint? FormaPagto { get; set; }
        }

        /// <summary>
        /// Finaliza uma cotação de compra, gerando compras (uma por fornecedor).
        /// </summary>
        public void Finalizar(uint codigoCotacaoCompra, Glass.Data.Model.CotacaoCompra.TipoCalculoCotacao tipoCalculo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var calculados = CalcularCotacao.Instance.CalcularPorFornecedor(transaction, codigoCotacaoCompra, tipoCalculo);

                    // Recupera as informações dos fornecedores, validando-as
                    Dictionary<uint, DadosFornecedor> fornecedores = new Dictionary<uint, DadosFornecedor>();

                    foreach (uint codigoFornecedor in calculados.Keys.Select(x => x.CodigoFornecedor).Distinct())
                    {
                        DadosFornecedor f = new DadosFornecedor()
                        {
                            Nome = FornecedorDAO.Instance.GetNome(transaction, codigoFornecedor),
                            FormaPagto = FornecedorDAO.Instance.ObtemTipoPagto(transaction, codigoFornecedor),
                            PlanoConta = FornecedorDAO.Instance.ObtemIdConta(transaction, codigoFornecedor)
                        };

                        if (f.FormaPagto == null)
                            throw new Exception("Configure a forma de pagamento padrão do fornecedor " + f.Nome);

                        if (f.PlanoConta == null)
                            throw new Exception("Configure o plano de conta padrão do fornecedor " + f.Nome);

                        fornecedores.Add(codigoFornecedor, f);
                    }

                    Glass.Data.Model.Compra compra;
                    Glass.Data.Model.ProdutosCompra produtoCompra;
                    uint idCompra;

                    foreach (var dadosFornec in calculados.Keys)
                    {
                        // Insere a compra
                        compra = new Glass.Data.Model.Compra()
                        {
                            IdFornec = dadosFornec.CodigoFornecedor,
                            IdLoja = UserInfo.GetUserInfo.IdLoja,
                            IdConta = fornecedores[dadosFornec.CodigoFornecedor].PlanoConta.Value,
                            IdFormaPagto = fornecedores[dadosFornec.CodigoFornecedor].FormaPagto.Value,
                            TipoCompra = dadosFornec.DatasParcelas.Length == 0 ? (int)Glass.Data.Model.Compra.TipoCompraEnum.AVista :
                                (int)Glass.Data.Model.Compra.TipoCompraEnum.APrazo,
                            Situacao = Glass.Data.Model.Compra.SituacaoEnum.Ativa,
                            DataFabrica = DateTime.Now.ObtemDataDiasUteis((int)dadosFornec.PrazoEntregaDias),
                        };

                        idCompra = CompraDAO.Instance.InsertFromCotacao(transaction, compra, codigoCotacaoCompra);

                        // Insere os produtos da compra
                        foreach (var produto in calculados[dadosFornec])
                        {
                            produtoCompra = new Glass.Data.Model.ProdutosCompra()
                            {
                                IdCompra = idCompra,
                                IdProd = produto.CodigoProduto,
                                Qtde = produto.QuantidadeProduto,
                                Valor = produto.CustoUnitarioProdutoFornecedor,
                                Total = produto.CustoTotalProdutoFornecedor,
                                Altura = produto.AlturaProduto,
                                Largura = produto.LarguraProduto,
                                TotM = produto.TotalM2Produto,
                                Espessura = ProdutoDAO.Instance.ObtemEspessura(transaction, (int)produto.CodigoProduto)
                            };

                            ProdutosCompraDAO.Instance.Insert(transaction, produtoCompra);
                        }

                        // Salva as parcelas da compra
                        if (dadosFornec.DatasParcelas.Length > 0)
                            CompraDAO.Instance.AlteraParcelas(transaction, idCompra, dadosFornec.DatasParcelas.Length, dadosFornec.DatasParcelas);
                    }

                    // Finaliza a cotação de compra
                    CotacaoCompraDAO.Instance.Finalizar(transaction, codigoCotacaoCompra, tipoCalculo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }
    }
}
