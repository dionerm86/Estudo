using System;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class CRUD : BaseFluxo<CRUD>
    {
        private CRUD() { }

        #region Ajax

        private static Ajax.ICRUD _ajax = null;

        public static Ajax.ICRUD Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.CRUD();

                return _ajax;
            }
        }

        #endregion

        #region CotacaoCompra

        #region Create

        /// <summary>
        /// Insere uma cotação de compra.
        /// </summary>
        /// <param name="cotacaoCompra"></param>
        public uint InserirCotacaoCompra(Entidade.CotacaoCompra cotacaoCompra)
        {
            cotacaoCompra.CodFuncCadastro = UserInfo.GetUserInfo.CodUser;
            cotacaoCompra.DataCadastro = DateTime.Now;
            cotacaoCompra.Situacao = Glass.Data.Model.CotacaoCompra.SituacaoEnum.Aberta;

            cotacaoCompra.Codigo = CotacaoCompraDAO.Instance.Insert(cotacaoCompra._cotacao);
            return cotacaoCompra.Codigo;
        }

        #endregion

        #region Read

        /// <summary>
        /// Busca uma cotação de compra.
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public Entidade.CotacaoCompra ObtemCotacaoCompra(uint codigo)
        {
            return new Entidade.CotacaoCompra(CotacaoCompraDAO.Instance.GetElementByPrimaryKey(codigo));
        }

        /// <summary>
        /// Busca uma lista de cotações de compra.
        /// </summary>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.CotacaoCompra[] ObtemCotacoesCompra(uint idCotacaoCompra, int situacao, string dataCadIni, 
            string dataCadFim, string sortExpression, int startRow, int pageSize)
        {
            var cotacoes = CotacaoCompraDAO.Instance.ObtemCotacoes(idCotacaoCompra, situacao, dataCadIni, dataCadFim, 
                sortExpression, startRow, pageSize).ToArray();

            return Array.ConvertAll(cotacoes, x => new Entidade.CotacaoCompra(x));
        }

        /// <summary>
        /// Retorna o número de cotações de compra cadastradas.
        /// </summary>
        /// <returns></returns>
        public int ObtemNumeroCotacoesCompra(uint idCotacaoCompra, int situacao, string dataCadIni, string dataCadFim)
        {
            return CotacaoCompraDAO.Instance.ObtemNumeroCotacoes(idCotacaoCompra, situacao, dataCadIni, dataCadFim);
        }

        #endregion

        #region Update

        /// <summary>
        /// Atualiza uma cotação de compra.
        /// </summary>
        /// <param name="cotacaoCompra"></param>
        public void AtualizarCotacaoCompra(Entidade.CotacaoCompra cotacaoCompra)
        {
            CotacaoCompraDAO.Instance.Update(cotacaoCompra._cotacao);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Exclui uma cotação de compra.
        /// </summary>
        /// <param name="cotacaoCompra"></param>
        public void ExcluirCotacaoCompra(Entidade.CotacaoCompra cotacaoCompra)
        {
            CotacaoCompraDAO.Instance.Delete(cotacaoCompra._cotacao);
        }

        #endregion

        #endregion

        #region ProdutoCotacaoCompra

        #region Create

        /// <summary>
        /// Insere um produto na cotação de compra.
        /// </summary>
        /// <param name="produtoCotacaoCompra"></param>
        public void InserirProdutoCotacaoCompra(Entidade.ProdutoCotacaoCompra produtoCotacaoCompra)
        {
            produtoCotacaoCompra.Codigo = ProdutoCotacaoCompraDAO.Instance.Insert(produtoCotacaoCompra._produto);
        }

        #endregion

        #region Read

        /// <summary>
        /// Retorna uma lista de produtos de uma cotação de compra.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.ProdutoCotacaoCompra[] ObtemProdutosCotacaoCompra(uint codigoCotacaoCompra,
            string sortExpression, int startRow, int pageSize)
        {
            var produtos = ProdutoCotacaoCompraDAO.Instance.ObtemProdutos(codigoCotacaoCompra, sortExpression, startRow, pageSize).ToArray();
            if (produtos.Length == 0)
                produtos = new Glass.Data.Model.ProdutoCotacaoCompra[] { new Glass.Data.Model.ProdutoCotacaoCompra() };

            return Array.ConvertAll(produtos, x => new Entidade.ProdutoCotacaoCompra(x));
        }

        /// <summary>
        /// Retorna o número de produtos de uma cotação de compra (valor mínimo: 1).
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <returns></returns>
        public int ObtemNumeroProdutosCotacaoCompra(uint codigoCotacaoCompra)
        {
            int numero = ObtemNumeroRealProdutosCotacaoCompra(codigoCotacaoCompra);
            return numero > 0 ? numero : 1;
        }

        /// <summary>
        /// Retorna o número real de produtos de uma cotação de compra.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <returns></returns>
        public int ObtemNumeroRealProdutosCotacaoCompra(uint codigoCotacaoCompra)
        {
            return ProdutoCotacaoCompraDAO.Instance.ObtemNumeroProdutos(codigoCotacaoCompra);
        }

        #endregion

        #region Update

        /// <summary>
        /// Atualiza um produto na cotação de compra.
        /// </summary>
        /// <param name="produtoCotacaoCompra"></param>
        public void AtualizarProdutoCotacaoCompra(Entidade.ProdutoCotacaoCompra produtoCotacaoCompra)
        {
            ProdutoCotacaoCompraDAO.Instance.Update(produtoCotacaoCompra._produto);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Exclui um produto de cotação de compra.
        /// </summary>
        /// <param name="produtoCotacaoCompra"></param>
        public void ExcluirProdutoCotacaoCompra(Entidade.ProdutoCotacaoCompra produtoCotacaoCompra)
        {
            ProdutoCotacaoCompraDAO.Instance.Delete(produtoCotacaoCompra._produto);
        }

        #endregion

        #endregion

        #region ProdutoFornecedorCotacaoCompra

        #region Create/Update

        /// <summary>
        /// Habilita um fornecedor de produto de cotação de compra.
        /// </summary>
        /// <param name="produtoFornecedorCotacaoCompra"></param>
        public void HabilitarProdutoFornecedorCotacaoCompra(
            Entidade.ProdutoFornecedorCotacaoCompra produtoFornecedorCotacaoCompra)
        {
            ProdutoFornecedorCotacaoCompraDAO.Instance.InsertOrUpdate(produtoFornecedorCotacaoCompra._produtoFornecedor);
        }

        #endregion

        #region Read

        /// <summary>
        /// Obtém os fornecedores de produtos de cotação de compra.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <param name="codigoFornecedor"></param>
        /// <param name="codigoProduto"></param>
        /// <param name="apenasCadastrados">Buscar apenas os fornecedores cadastrados ou todos os possíveis?</param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.ProdutoFornecedorCotacaoCompra[] ObtemProdutosFornecedorCotacaoCompra(uint codigoCotacaoCompra,
            uint codigoFornecedor, uint codigoProduto, bool apenasCadastrados, string sortExpression, int startRow, int pageSize)
        {
            var produtos = ProdutoFornecedorCotacaoCompraDAO.Instance.ObtemProdutosFornecedorCotacao(codigoCotacaoCompra,
                codigoFornecedor, codigoProduto, apenasCadastrados, sortExpression, startRow, pageSize);

            return Array.ConvertAll(produtos, x => new Entidade.ProdutoFornecedorCotacaoCompra(x));
        }

        public int ObtemNumeroProdutosFornecedorCotacaoCompra(uint codigoCotacaoCompra, uint codigoFornecedor, 
            uint codigoProduto, bool apenasCadastrados)
        {
            return ProdutoFornecedorCotacaoCompraDAO.Instance.ObtemNumeroProdutosFornecedorCotacao(codigoCotacaoCompra,
                codigoFornecedor, codigoProduto, apenasCadastrados);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Desabilita o fornecedor para um produto de cotação de compra.
        /// </summary>
        /// <param name="produtoFornecedorCotacaoCompra"></param>
        public void DesabilitarProdutoFornecedorCotacaoCompra(
            Entidade.ProdutoFornecedorCotacaoCompra produtoFornecedorCotacaoCompra)
        {
            ProdutoFornecedorCotacaoCompraDAO.Instance.Delete(produtoFornecedorCotacaoCompra._produtoFornecedor);
        }

        #endregion

        #endregion
    }
}
