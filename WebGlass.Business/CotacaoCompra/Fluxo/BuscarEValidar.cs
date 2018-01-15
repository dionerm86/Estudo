using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass;

namespace WebGlass.Business.CotacaoCompra.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public bool PodeEditar(uint codigoCotacaoCompra)
        {
            int situacao = CotacaoCompraDAO.Instance.ObtemSituacao(codigoCotacaoCompra);
            return situacao == (int)Glass.Data.Model.CotacaoCompra.SituacaoEnum.Aberta;
        }

        /// <summary>
        /// Busca os produtos cadastrados na cotação de compra para o filtro.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <returns></returns>
        public GenericModel[] GetProdutos(uint codigoCotacaoCompra)
        {
            var produtos = (from p in ProdutoCotacaoCompraDAO.Instance.ObtemProdutos(null, codigoCotacaoCompra)
                            select p.IdProd).Distinct();

            List<GenericModel> itens = new List<GenericModel>(Array.ConvertAll(produtos.ToArray(),
                x => new GenericModel(x, ProdutoDAO.Instance.ObtemDescricao((int)x))));

            itens.Sort((x, y) => String.Compare(x.Descr, y.Descr));

            itens.Insert(0, new GenericModel(null, "Todos"));
            return itens.ToArray();
        }

        /// <summary>
        /// Busca os fornecedores cadastrados na cotação de compra para o filtro.
        /// </summary>
        /// <param name="codigoCotacaoCompra"></param>
        /// <returns></returns>
        public GenericModel[] GetFornecedores(uint codigoCotacaoCompra)
        {
            var fornecedores = (from p in ProdutoFornecedorCotacaoCompraDAO.Instance.
                                    ObtemProdutosFornecedorCotacao(null, codigoCotacaoCompra, 0, 0, false)
                                select p.IdFornec).Distinct();

            List<GenericModel> itens = new List<GenericModel>(Array.ConvertAll(fornecedores.ToArray(),
                x => new GenericModel(x, FornecedorDAO.Instance.GetNome(x))));

            itens.Sort((x, y) => String.Compare(x.Descr, y.Descr));

            itens.Insert(0, new GenericModel(null, "Todos"));
            return itens.ToArray();
        }
    }
}
