using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model.Calculos
{
    class DadosBaixaEstoqueDTO : IDadosBaixaEstoque
    {
        private static readonly CacheMemoria<IEnumerable<ProdutoBaixaEstoque>, int> produtosBaixaEstoque;

        private readonly Lazy<IEnumerable<ProdutoBaixaEstoque>> produtoBaixaEstoque;

        static DadosBaixaEstoqueDTO()
        {
            produtosBaixaEstoque = new CacheMemoria<IEnumerable<ProdutoBaixaEstoque>, int>("produtosBaixaEstoque");
        }

        internal DadosBaixaEstoqueDTO(GDASession sessao, Lazy<Produto> produto)
        {
            produtoBaixaEstoque = new Lazy<IEnumerable<Model.ProdutoBaixaEstoque>>(() =>
                ObterProdutosBaixaEstoque(sessao, produto.Value.IdProd));
        }

        public IEnumerable<float> QuantidadesBaixaEstoque()
        {
            return produtoBaixaEstoque.Value
                .Select(produto => produto.Qtde);
        }

        private IEnumerable<ProdutoBaixaEstoque> ObterProdutosBaixaEstoque(GDASession sessao, int idProduto)
        {
            var produtosBaixa = produtosBaixaEstoque.RecuperarDoCache(idProduto);

            if (produtosBaixa == null)
            {
                try
                {
                    produtosBaixa = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, (uint)idProduto)
                        ?? new ProdutoBaixaEstoque[0];
                }
                catch
                {
                    produtosBaixa = new ProdutoBaixaEstoque[0];
                }

                produtosBaixaEstoque.AtualizarItemNoCache(produtosBaixa, idProduto);
            }

            return produtosBaixa;
        }
    }
}
