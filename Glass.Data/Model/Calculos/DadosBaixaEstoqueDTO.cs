using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model.Calculos
{
    class DadosBaixaEstoqueDTO : BaseCalculoDTO, IDadosBaixaEstoque
    {
        private static readonly CacheMemoria<List<ProdutoBaixaEstoque>, int> cacheProdutosBaixaEstoque;

        private readonly Lazy<List<ProdutoBaixaEstoque>> produtosBaixaEstoque;

        static DadosBaixaEstoqueDTO()
        {
            cacheProdutosBaixaEstoque = new CacheMemoria<List<ProdutoBaixaEstoque>, int>("produtosBaixaEstoque");
        }

        internal DadosBaixaEstoqueDTO(GDASession sessao, Lazy<Produto> produto)
        {
            produtosBaixaEstoque = ObterProdutosBaixaEstoque(sessao, produto.Value.IdProd);
        }

        public IEnumerable<float> QuantidadesBaixaEstoque()
        {
            return produtosBaixaEstoque.Value
                .Select(produto => produto.Qtde);
        }

        private Lazy<List<ProdutoBaixaEstoque>> ObterProdutosBaixaEstoque(GDASession sessao, int idProduto)
        {
            return ObterUsandoCache(
                cacheProdutosBaixaEstoque,
                idProduto,
                () => ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, (uint)idProduto).ToList()
            );
        }
    }
}
