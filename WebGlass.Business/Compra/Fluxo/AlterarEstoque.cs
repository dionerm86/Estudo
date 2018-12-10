using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using WebGlass.Business.Compra.Entidade;

namespace WebGlass.Business.Compra.Fluxo
{
    public sealed class AlterarEstoque : BaseFluxo<AlterarEstoque>
    {
        private AlterarEstoque() { }

        static volatile object _aumentarEstoqueLock = new object();

        public void AumentarEstoque(uint idLoja, IEnumerable<DadosAumentarEstoque> dadosAumentarEstoque)
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

                        foreach (DadosAumentarEstoque d in dadosAumentarEstoque)
                        {
                            var prodCompra = ProdutosCompraDAO.Instance.GetElementByPrimaryKey(transaction, d.IdProdCompra);

                            // Se a quantidade a ser marcada como saída for maior do que o máximo, não permite marcar saída
                            if (d.QtdeAumentar > prodCompra.Qtde - prodCompra.QtdeEntrada)
                                throw new Exception("Operação cancelada. O produto " + d.DescricaoProduto + " teve uma entrada maior do que sua quantidade.");
                            else
                            {
                                prodCompra.QtdMarcadaEntrada = d.QtdeAumentar;

                                lstProdCompra.Add(prodCompra);
                            }
                        }

                        MovEstoqueDAO.Instance.CreditaEstoqueManualCompra(transaction, idLoja, lstProdCompra[0].IdCompra, lstProdCompra);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("AumentarEstoque Compra", ex);
                        throw;
                    }
                }
            }
        }
    }
}
