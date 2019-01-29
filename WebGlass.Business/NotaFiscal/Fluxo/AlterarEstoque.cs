using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using WebGlass.Business.NotaFiscal.Entidade;

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
                            prodNf = ProdutosNfDAO.Instance.GetElement(transaction, d.IdProdNf);

                            if (prodNf == null || d.IdProdNf == 0)
                                throw new Exception("Não foi possível recuperar o produto da nota para creditar o estoque.");

                            // Se a quantidade a ser marcada como saída for maior do que o máximo, não permite marcar saída
                            if (d.QtdeAumentar > prodNf.Qtde - prodNf.QtdeEntrada)
                                throw new Exception("Operação cancelada. O produto " + d.DescricaoProduto + " teve uma entrada maior do que sua quantidade.");
                            else
                            {
                                prodNf.QtdMarcadaEntrada = d.QtdeAumentar;
                                lstProdNf.Add(prodNf);
                            }
                        }

                        if (!lstProdNf.Any())
                        {
                            throw new Exception("Falha ao calcular a quantidade dos produtos a ser creditada no estoque.");
                        }

                        MovEstoqueDAO.Instance.CreditaEstoqueNotaFiscalManual(transaction, (int)idLoja, (int)lstProdNf[0].IdNf, lstProdNf);

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
