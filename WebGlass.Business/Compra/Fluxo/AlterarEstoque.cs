using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using WebGlass.Business.Compra.Entidade;
using Glass.Configuracoes;

namespace WebGlass.Business.Compra.Fluxo
{
    public sealed class AlterarEstoque : BaseFluxo<AlterarEstoque>
    {
        private AlterarEstoque() { }

        public void BaixarEstoque(uint idCompra)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Verifica se o Pedido possui produtos
                    if (ProdutosCompraDAO.Instance.CountInCompra(transaction, idCompra) == 0)
                        throw new Exception("Inclua pelo menos um produto na compra para creditar o estoque.");

                    // Credita o estoque pela compra apenas se a empresa não credita manualmente
                    if (!EstoqueConfig.EntradaEstoqueManual)
                    {
                        var compra = CompraDAO.Instance.GetElementByPrimaryKey(transaction, idCompra);

                        CompraDAO.Instance.CreditarEstoqueCompra(transaction, compra);
                    }

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

        static volatile object _aumentarEstoqueLock = new object();

        public void AumentarEstoque(uint idLoja, IEnumerable<DadosAumentarEstoque> dadosAumentarEstoque, bool manual)
        {
            lock (_aumentarEstoqueLock)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var usuario = UserInfo.GetUserInfo;

                        if (usuario.TipoUsuario != (uint)Utils.TipoFuncionario.AuxAlmoxarifado &&
                            !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                        {
                            throw new Exception("Você não tem permissão para marcar entrada de produtos.");
                        }

                        List<ProdutosCompra> lstProdCompra = new List<ProdutosCompra>();
                        ProdutosCompra prodCompra;

                        foreach (DadosAumentarEstoque d in dadosAumentarEstoque)
                        {
                            prodCompra = ProdutosCompraDAO.Instance.GetElementByPrimaryKey(transaction, d.IdProdCompra);

                            // Se a quantidade a ser marcada como saída for maior do que o máximo, não permite marcar saída
                            if (d.QtdeAumentar > prodCompra.Qtde - prodCompra.QtdeEntrada)
                                throw new Exception("Operação cancelada. O produto " + d.DescricaoProduto + " teve uma entrada maior do que sua quantidade.");
                            else
                            {
                                int tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodCompra.IdProd);
                                bool m2 = tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                                // coloca o metro linear baixado no campo m²
                                if (prodCompra.IdProd > 0 && (tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6))
                                {
                                    prodCompra.TotM = d.QtdeAumentar * prodCompra.Altura;
                                }
                                // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                                else if (m2)
                                    prodCompra.TotM = Glass.Global.CalculosFluxo.ArredondaM2Compra(prodCompra.Largura, (int)prodCompra.Altura, (int)d.QtdeAumentar);

                                prodCompra.QtdMarcadaEntrada = d.QtdeAumentar;

                                lstProdCompra.Add(prodCompra);
                            }
                        }

                        uint idEntradaEstoque = EntradaEstoqueDAO.Instance.GetNewEntradaEstoque(transaction, idLoja, lstProdCompra[0].IdCompra, null, manual, (int)usuario.CodUser);

                        // Marca saída dos produtos
                        foreach (ProdutosCompra p in lstProdCompra)
                        {
                            if (p.QtdMarcadaEntrada == 0)
                                continue;

                            // Marca quantos produtos do pedido foi marcado como saída
                            ProdutosCompraDAO.Instance.MarcarEntrada(transaction, p.IdProdCompra, p.QtdMarcadaEntrada, idEntradaEstoque);

                            int tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdProd);
                            float qtdCredito = p.QtdMarcadaEntrada;

                            if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                                tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                                qtdCredito *= p.Altura;

                            bool m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                            // Credita no estoque da loja
                            MovEstoqueDAO.Instance.CreditaEstoqueCompra(transaction, p.IdProd, idLoja, p.IdCompra, p.IdProdCompra,
                                (decimal)(m2 ? p.TotM : qtdCredito));
                        }

                        CompraDAO.Instance.MarcaEstoqueBaixado(transaction, lstProdCompra[0].IdCompra);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("AumentarEstoque Compra", ex);
                        throw ex;
                    }
                }
            }
        }
    }
}
