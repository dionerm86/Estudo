using Glass.Data.RelModel;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public static class ProdutosReposicaoDAO
    {
        public static ProdutosReposicao[] GetByProdutosPedido(ProdutosPedido[] produtos)
        {
            int i = 0;
            Glass.Data.RelModel.ProdutosReposicao[] prodRepos = new Glass.Data.RelModel.ProdutosReposicao[produtos.Length];
            foreach (ProdutosPedido p in produtos)
                prodRepos[i++] = new Glass.Data.RelModel.ProdutosReposicao(p);

            return prodRepos;
        }
    }
}
