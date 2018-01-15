using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos detalhes da reposição de peça.
    /// </summary>
    public class DetalhesReposicaoPeca : Negocios.IDetalhesReposicaoPeca
    {
        /// <summary>
        /// Recupera os detalhes referentes às reposições de peça.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.DetalhesReposicaoPeca> PesquisarDetalhesReposicaoPeca(int idProdPedProducao)
        {
            var lista = new List<Entidades.DetalhesReposicaoPeca>();

            lista.AddRange(
                SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.ProdutoPedidoProducao>("ppp")
                    .InnerJoin<Glass.Data.Model.Funcionario>("ppp.IdFuncRepos=f.IdFunc", "f")
                    .InnerJoin<Glass.Data.Model.Setor>("ppp.IdSetorRepos=s.IdSetor", "s")
                    .Where("ppp.IdProdPedProducao=?id")
                        .Add("?id", idProdPedProducao)
                    .Select(
                        @"ppp.IdProdPedProducao, ppp.NumEtiqueta, ppp.DadosReposicaoPeca, f.Nome as FuncRepos, 
                          s.Descricao as SetorRepos, ppp.DataRepos")
                    .ToVirtualResult<Entidades.DetalhesReposicaoPeca>());

            lista.AddRange(
                SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.DadosReposicao>("dr")
                    .InnerJoin<Glass.Data.Model.ProdutoPedidoProducao>("dr.IdProdPedProducao=ppp.IdProdPedProducao", "ppp")
                    .InnerJoin<Glass.Data.Model.Funcionario>("dr.IdFuncRepos=f.IdFunc", "f")
                    .InnerJoin<Glass.Data.Model.Setor>("dr.IdSetorRepos=s.IdSetor", "s")
                    .Where("dr.IdProdPedProducao=?id")
                        .Add("?id", idProdPedProducao)
                    .Select(
                        @"dr.IdProdPedProducao, ppp.NumEtiqueta, dr.DadosReposicaoPeca, f.Nome as FuncRepos, 
                          s.Descricao as SetorRepos, dr.DataRepos")
                    .ToVirtualResult<Entidades.DetalhesReposicaoPeca>());

            return lista.OrderByDescending(x => x.DataRepos).ToList();
        }
    }
}
