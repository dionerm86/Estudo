using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model.Calculos
{
    class DadosGrupoSubgrupoDTO : IDadosGrupoSubgrupo
    {
        private static readonly CacheMemoria<GrupoProd, int> grupos;
        private static readonly CacheMemoria<SubgrupoProd, int> subgrupos;
        
        private readonly Lazy<GrupoProd> grupoProduto;
        private readonly Lazy<SubgrupoProd> subgrupoProduto;

        static DadosGrupoSubgrupoDTO()
        {
            grupos = new CacheMemoria<GrupoProd, int>("grupos");
            subgrupos = new CacheMemoria<SubgrupoProd, int>("subgrupos");
        }

        internal DadosGrupoSubgrupoDTO(GDASession sessao, Lazy<Produto> produto)
        {
            grupoProduto = new Lazy<GrupoProd>(() => ObterGrupo(sessao, produto.Value.IdGrupoProd));
            subgrupoProduto = new Lazy<SubgrupoProd>(() => ObterSubgrupo(sessao, produto.Value.IdSubgrupoProd));
        }

        public string DescricaoSubgrupo()
        {
            return subgrupoProduto.Value.Descricao;
        }

        public bool ProdutoDeProducao()
        {
            return subgrupoProduto.Value.IdGrupoProd == (int)NomeGrupoProd.Vidro
                && subgrupoProduto.Value.ProdutosEstoque;
        }

        public bool IsVidroTemperado()
        {
            return subgrupoProduto.Value.IsVidroTemperado;
        }

        public bool ProdutoEVidro()
        {
            return grupoProduto.Value.IdGrupoProd == (int)NomeGrupoProd.Vidro;
        }

        public bool ProdutoEAluminio()
        {
            return grupoProduto.Value.IdGrupoProd == (int)NomeGrupoProd.Alumínio;
        }

        public TipoSubgrupoProd TipoSubgrupo()
        {
            return subgrupoProduto.Value.TipoSubgrupo;
        }

        public TipoCalculoGrupoProd TipoCalculo(bool fiscal = false)
        {
            TipoCalculoGrupoProd? tipoCalculoFiscal = subgrupoProduto != null
                ? subgrupoProduto.Value.TipoCalculoNf ?? grupoProduto.Value.TipoCalculoNf
                : grupoProduto.Value.TipoCalculoNf;

            TipoCalculoGrupoProd? tipoCalculo = subgrupoProduto != null
                ? subgrupoProduto.Value.TipoCalculo
                : grupoProduto.Value.TipoCalculo;

            var tipoCalc = fiscal
                ? tipoCalculoFiscal ?? tipoCalculo
                : tipoCalculo;

            return tipoCalc ?? TipoCalculoGrupoProd.Qtd;
        }

        private GrupoProd ObterGrupo(GDASession sessao, int idGrupoProd)
        {
            if (idGrupoProd == 0)
                return new GrupoProd();

            var grupoProdutoCache = grupos.RecuperarDoCache(idGrupoProd);

            if (grupoProdutoCache == null)
            {
                try
                {
                    grupoProdutoCache = GrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idGrupoProd)
                        ?? new GrupoProd();
                }
                catch
                {
                    grupoProdutoCache = new GrupoProd();
                }

                grupos.AtualizarItemNoCache(grupoProdutoCache, idGrupoProd);
            }

            return grupoProdutoCache;
        }

        private SubgrupoProd ObterSubgrupo(GDASession sessao, int? idSubgrupoProd)
        {
            if ((idSubgrupoProd ?? 0) == 0)
                return new SubgrupoProd();

            var subgrupoProdutoCache = subgrupos.RecuperarDoCache(idSubgrupoProd.Value);

            if (subgrupoProdutoCache == null)
            {
                try
                {
                    subgrupoProdutoCache = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idSubgrupoProd.Value)
                        ?? new SubgrupoProd();
                }
                catch
                {
                    subgrupoProdutoCache = new SubgrupoProd();
                }

                subgrupos.AtualizarItemNoCache(subgrupoProdutoCache, idSubgrupoProd.Value);
            }

            return subgrupoProdutoCache;
        }
    }
}
