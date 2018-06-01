using Glass.Global.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do provedor de ICMS do produto por UF.
    /// </summary>
    public class ProvedorIcmsProdutoUf : Entidades.IProvedorIcmsProdutoUf
    {
        #region Métodos Públicos

        /// <summary>
        /// Busca o valor do ICMS por produto e UF.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja">Loja que será usada na pesquisa.</param>
        /// <param name="fornecedor">Fornecedor que será usado na pesquisa.</param>
        /// <param name="cliente">Cliente que será usado na pesquisa</param>
        /// <returns></returns>
        public float ObterIcmsPorProduto(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente)
        {
            // Essa implemetação é apenas um wrapper para a DAO
            using (var session = new GDA.GDASession())
                return Data.DAL.IcmsProdutoUfDAO.Instance.ObterIcmsPorProduto(session, 
                    (uint)produto.IdProd, (uint)(loja?.IdLoja ?? 0), (uint)(fornecedor?.IdFornec ?? 0), (uint)(cliente?.IdCli ?? 0));
        }

        /// <summary>
        /// Busca o valor do FCP por produto e UF.
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public float ObterFCPPorProduto(Produto produto, Loja loja, Fornecedor fornecedor, Cliente cliente)
        {
            // Essa implemetação é apenas um wrapper para a DAO
            using (var session = new GDA.GDASession())
                return Data.DAL.IcmsProdutoUfDAO.Instance.ObterFCPPorProduto(session,
                    (uint)produto.IdProd, (uint)(loja?.IdLoja ?? 0), (uint)(fornecedor?.IdFornec ?? 0), (uint)(cliente?.IdCli ?? 0));
        }

        #endregion
    }
}