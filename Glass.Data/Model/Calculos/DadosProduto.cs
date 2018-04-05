using Glass.Data.DAL;
using Glass.Data.Helper.Calculos.Cache;
using Glass.Configuracoes;

namespace Glass.Data.Model.Calculos
{
    class DadosProduto : IDadosProduto
    {
        private readonly CacheCalculo<Produto, int> produtos;
        private readonly CacheCalculo<SubgrupoProd, int> subgrupos;

        internal DadosProduto()
        {
            produtos = new CacheCalculo<Produto, int>(
                "produtos",
                produto => produto.IdProd
            );

            subgrupos = new CacheCalculo<SubgrupoProd, int>(
                "subgrupos",
                subgrupo => subgrupo.IdSubgrupoProd
            );
        }

        public bool CalcularAreaMinima(IProdutoCalculo produto, IContainerCalculo container,
            int numeroBeneficiamentos)
        {
            bool ativarAreaMinima = ObtemProduto(produto).AtivarAreaMinima &&
                container.Cliente.CobrarAreaMinima;

            if (PedidoConfig.DadosPedido.CalcularAreaMinimaApenasVidroBeneficiado)
            {
                if (!ProdutoEVidro(produto) || !ativarAreaMinima)
                    return false;

                if (ObtemSubgrupo(produto).IsVidroTemperado)
                    return true;

                return produto.Redondo || numeroBeneficiamentos > 0;
            }

            return ativarAreaMinima;
        }

        public float AreaMinima(IProdutoCalculo produto)
        {
            return ObtemProduto(produto)
                .AreaMinima;
        }

        public int IdGrupoProd(IProdutoCalculo produto)
        {
            return ObtemProduto(produto)
                .IdGrupoProd;
        }

        public string Descricao(IProdutoCalculo produto)
        {
            return ObtemProduto(produto)
                .Descricao;
        }

        public bool ProdutoDeProducao(IProdutoCalculo produto)
        {
            var subgrupo = ObtemSubgrupo(produto);
            return subgrupo.IdGrupoProd == (int)NomeGrupoProd.Vidro
                && subgrupo.ProdutosEstoque;
        }

        public bool ProdutoEVidro(IProdutoCalculo produto)
        {
            return IdGrupoProd(produto) == (int)NomeGrupoProd.Vidro;
        }

        public bool ProdutoEAluminio(IProdutoCalculo produto)
        {
            return IdGrupoProd(produto) == (int)NomeGrupoProd.Alumínio;
        }

        public string DescricaoSubgrupo(IProdutoCalculo produto)
        {
            return ObtemSubgrupo(produto)
                .Descricao;
        }

        private Produto ObtemProduto(IProdutoCalculo produtoCalculo)
        {
            var idProduto = (int)produtoCalculo.IdProduto;
            var produto = produtos.RecuperarDoCache(idProduto);

            if (produto == null)
            {
                produto = ProdutoDAO.Instance.GetElementByPrimaryKey(idProduto);
                produtos.AtualizarItemNoCache(produto);
            }

            return produto;
        }

        private SubgrupoProd ObtemSubgrupo(IProdutoCalculo produto)
        {
            var idSubgrupo = ObtemProduto(produto)
                .IdSubgrupoProd;

            if (!idSubgrupo.HasValue)
                return new SubgrupoProd();

            var subgrupo = subgrupos.RecuperarDoCache(idSubgrupo.Value);

            if (subgrupo == null)
            {
                subgrupo = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(idSubgrupo.Value);
                subgrupos.AtualizarItemNoCache(subgrupo);
            }

            return subgrupo;
        }
    }
}
