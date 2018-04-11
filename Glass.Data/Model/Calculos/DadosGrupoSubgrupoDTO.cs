using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model.Calculos
{
    class DadosGrupoSubgrupoDTO : BaseCalculoDTO, IDadosGrupoSubgrupo
    {
        private static readonly CacheMemoria<GrupoProd, int> cacheGrupos;
        private static readonly CacheMemoria<SubgrupoProd, int> cacheSubgrupos;
        
        private readonly Lazy<GrupoProd> grupoProduto;
        private readonly Lazy<SubgrupoProd> subgrupoProduto;

        static DadosGrupoSubgrupoDTO()
        {
            cacheGrupos = new CacheMemoria<GrupoProd, int>("cacheGrupos");
            cacheSubgrupos = new CacheMemoria<SubgrupoProd, int>("cacheSubgrupos");
        }

        internal DadosGrupoSubgrupoDTO(GDASession sessao, Lazy<Produto> produto)
        {
            grupoProduto = ObterGrupo(sessao, produto.Value.IdGrupoProd);
            subgrupoProduto = ObterSubgrupo(sessao, produto.Value.IdSubgrupoProd);
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

        private Lazy<GrupoProd> ObterGrupo(GDASession sessao, int idGrupoProd)
        {
            if (idGrupoProd == 0)
                return new Lazy<GrupoProd>(() => new GrupoProd());

            return ObterUsandoCache(
                cacheGrupos,
                idGrupoProd,
                () => GrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idGrupoProd)
            );
        }

        private Lazy<SubgrupoProd> ObterSubgrupo(GDASession sessao, int? idSubgrupoProd)
        {
            if ((idSubgrupoProd ?? 0) == 0)
                return new Lazy<SubgrupoProd>(() => new SubgrupoProd());

            return ObterUsandoCache(
                cacheSubgrupos,
                idSubgrupoProd.Value,
                () => SubgrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idSubgrupoProd.Value)
            );
        }
    }
}
