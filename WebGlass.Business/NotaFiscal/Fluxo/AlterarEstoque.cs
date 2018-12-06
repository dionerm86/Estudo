using System;
using System.Collections.Generic;
using System.Linq;
using WebGlass.Business.NotaFiscal.Entidade;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass;

namespace WebGlass.Business.NotaFiscal.Fluxo
{
    public sealed class AlterarEstoque : BaseFluxo<AlterarEstoque>
    {
        private AlterarEstoque() { }

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

                        if (UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.AuxAlmoxarifado &&
                            !Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque))
                        {
                            throw new Exception("Você não tem permissão para marcar entrada de produtos.");
                        }

                        if (dadosAumentarEstoque == null || dadosAumentarEstoque.Count() == 0)
                            throw new Exception("Falha ao recuperar os produtos para creditar o estoque.");

                        var lstProdNf = new List<ProdutosNf>();
                        ProdutosNf prodNf;

                        foreach (DadosAumentarEstoque d in dadosAumentarEstoque)
                        {
                            prodNf = ProdutosNfDAO.Instance.GetElementByPrimaryKey(transaction, d.IdProdNf);

                            if (prodNf == null || d.IdProdNf == 0)
                                throw new Exception("Não foi possível recuperar o produto da nota para creditar o estoque.");

                            // Se a quantidade a ser marcada como saída for maior do que o máximo, não permite marcar saída
                            if (d.QtdeAumentar > prodNf.Qtde - prodNf.QtdeEntrada)
                                throw new Exception("Operação cancelada. O produto " + d.DescricaoProduto + " teve uma entrada maior do que sua quantidade.");
                            else
                            {
                                int tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodNf.IdProd, false);

                                // Se a empresa trabalha com venda de alumínio no metro e se produto for alumínio, 
                                // coloca o metro linear baixado no campo m²
                                if (prodNf.IdProd > 0 && (tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6))
                                {
                                    prodNf.TotM = d.QtdeAumentar * prodNf.Altura;
                                }
                                // Se produto for calculado por m², dá baixa somente no m² da qtde de peças que foram dado baixa
                                else if (tipoCalc == (int)TipoCalculoGrupoProd.M2 || tipoCalc == (int)TipoCalculoGrupoProd.M2Direto)
                                    prodNf.TotM = Glass.Global.CalculosFluxo.ArredondaM2(transaction, prodNf.Largura, (int)prodNf.Altura, d.QtdeAumentar, (int)prodNf.IdProd, false, 0, true);

                                prodNf.QtdMarcadaEntrada = d.QtdeAumentar;

                                lstProdNf.Add(prodNf);
                            }
                        }

                        if (lstProdNf.Count == 0)
                            throw new Exception("Falha ao calcular a quantidade dos produtos a ser creditada no estoque.");

                        var numeroNFe = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(transaction, lstProdNf[0].IdNf.ToString()).StrParaUint();
                        var idEntradaEstoque = EntradaEstoqueDAO.Instance.GetNewEntradaEstoque(transaction, idLoja, null, numeroNFe, manual,
                            (int)UserInfo.GetUserInfo.CodUser);

                        // Marca saída dos produtos
                        foreach (var p in lstProdNf)
                        {
                            if (p.QtdMarcadaEntrada == 0)
                                continue;

                            // Marca quantos produtos do pedido foi marcado como saída
                            ProdutosNfDAO.Instance.MarcarEntrada(transaction, p.IdProdNf, p.QtdMarcadaEntrada, idEntradaEstoque);

                            int tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdProd, false);
                            float qtdCredito = p.QtdMarcadaEntrada;

                            if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                                tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                                qtdCredito *= p.Altura;

                            bool m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                            // Credita no estoque da loja
                            MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscal(transaction, p.IdProd, idLoja, p.IdNf, p.IdProdNf,
                                (decimal)(m2 ? p.TotM : qtdCredito));
                        }

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("AumentarEstoque NFE", ex);
                        throw;
                    }
                }
            }
        }
    }
}
