using GDA;
using Glass.Comum.Cache;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model.Calculos
{
    class DadosGrupoSubgrupoDTO : IDadosGrupoSubgrupo
    {
        [ThreadStatic]
        private static readonly CacheMemoria<GrupoProd, int> grupos;

        [ThreadStatic]
        private static readonly CacheMemoria<SubgrupoProd, int> subgrupos;
        
        private readonly GrupoProd grupoProduto;
        private readonly SubgrupoProd subgrupoProduto;

        static DadosGrupoSubgrupoDTO()
        {
            grupos = new CacheMemoria<GrupoProd, int>("grupos");
            subgrupos = new CacheMemoria<SubgrupoProd, int>("subgrupos");
        }

        internal DadosGrupoSubgrupoDTO(GDASession sessao, int idGrupoProd, int? idSubgrupoProd)
        {
            grupoProduto = ObterGrupo(sessao, idGrupoProd);
            subgrupoProduto = ObterSubgrupo(sessao, idSubgrupoProd);
        }

        public string DescricaoSubgrupo()
        {
            return subgrupoProduto.Descricao;
        }

        public bool ProdutoDeProducao()
        {
            return subgrupoProduto.IdGrupoProd == (int)NomeGrupoProd.Vidro
                && subgrupoProduto.ProdutosEstoque;
        }

        public bool IsVidroTemperado()
        {
            return subgrupoProduto.IsVidroTemperado;
        }

        public bool ProdutoEVidro()
        {
            return grupoProduto.IdGrupoProd == (int)NomeGrupoProd.Vidro;
        }

        public bool ProdutoEAluminio()
        {
            return grupoProduto.IdGrupoProd == (int)NomeGrupoProd.Alumínio;
        }

        public TipoSubgrupoProd TipoSubgrupo()
        {
            return subgrupoProduto.TipoSubgrupo;
        }

        public TipoCalculoGrupoProd TipoCalculo(bool fiscal = false)
        {
            TipoCalculoGrupoProd? tipoCalculoFiscal = subgrupoProduto != null
                ? subgrupoProduto.TipoCalculoNf ?? grupoProduto.TipoCalculoNf
                : grupoProduto.TipoCalculoNf;

            TipoCalculoGrupoProd? tipoCalculo = subgrupoProduto != null
                ? subgrupoProduto.TipoCalculo
                : grupoProduto.TipoCalculo;

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
                    grupoProdutoCache = GrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idGrupoProd);
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
                    subgrupoProdutoCache = SubgrupoProdDAO.Instance.GetElementByPrimaryKey(sessao, idSubgrupoProd.Value);
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
