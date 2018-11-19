// <copyright file="GetNaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de naturezas de operação.
    /// </summary>
    public partial class NaturezasOperacaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de naturezas de operação.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das naturezas de operação.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Naturezas de operação sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(204, "Naturezas de operação não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Naturezas de operação paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaNaturezasOperacao([FromUri] Models.Cfops.V1.NaturezasOperacao.Lista.FiltroDto filtro)
        {
            filtro = filtro ?? new Models.Cfops.V1.NaturezasOperacao.Lista.FiltroDto();

            if (filtro.IdCfop == null || filtro.IdCfop == 0)
            {
                return this.ErroValidacao("O identificador do CFOP é obrigatório para buscar as naturezas de operação.");
            }

            var regras = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                .PesquisarNaturezasOperacao(filtro.IdCfop.GetValueOrDefault());

            ((Colosoft.Collections.IVirtualList)regras).Configure(filtro.NumeroRegistros);
            ((Colosoft.Collections.ISortableCollection)regras).ApplySort(filtro.ObterTraducaoOrdenacao());

            return this.ListaPaginada(
                regras
                    .Skip(filtro.ObterPrimeiroRegistroRetornar())
                    .Take(filtro.NumeroRegistros)
                    .Select(c => new Models.Cfops.V1.NaturezasOperacao.Lista.ListaDto(c)),
                filtro,
                () => regras.Count);
        }

        /// <summary>
        /// Recupera as naturezas de operação para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Naturezas de operação encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Naturezas de operação não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var naturezas = NaturezaOperacaoDAO.Instance.ObtemTodosOrdenados(sessao)
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdNaturezaOperacao,
                        Nome = c.CodCfop,
                    });

                return this.Lista(naturezas);
            }
        }

        /// <summary>
        /// Recupera CSOSN's para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("csosns")]
        [SwaggerResponse(200, "CSOSN's encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CSOSN's não encontrados.")]
        public IHttpActionResult ObterCsosns()
        {
            using (var sessao = new GDATransaction())
            {
                var csosns = Data.Helper.DataSources.Instance.GetCSOSN()
                    .Select(c => new CodigoNomeDto()
                    {
                        Codigo = c.Id?.ToString(),
                        Nome = c.Descr,
                    });

                return this.Lista(csosns);
            }
        }

        /// <summary>
        /// Recupera CST's de ICMS para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("cstsIcms")]
        [SwaggerResponse(200, "CST's de ICMS encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CST's de ICMS não encontrados.")]
        public IHttpActionResult ObterCstsIcms()
        {
            using (var sessao = new GDATransaction())
            {
                var cstsIcms = Data.EFD.DataSourcesEFD.Instance.GetCstIcms()
                    .Select(c => new CodigoNomeDto()
                    {
                        Codigo = c.Key,
                        Nome = c.Value,
                    });

                return this.Lista(cstsIcms);
            }
        }

        /// <summary>
        /// Recupera CST's de IPI para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("cstsIpi")]
        [SwaggerResponse(200, "CST's de IPI encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CST's de IPI não encontrados.")]
        public IHttpActionResult ObterCstsIpi()
        {
            using (var sessao = new GDATransaction())
            {
                var cstsIpi = new ConversorEnum<Data.Model.ProdutoCstIpi>()
                    .ObterTraducao();

                return this.Lista(cstsIpi);
            }
        }

        /// <summary>
        /// Recupera CST's de PIS/COFINS para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos itens encontrados.</returns>
        [HttpGet]
        [Route("cstsPisCofins")]
        [SwaggerResponse(200, "CST's de PIS/COFINS encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CST's de PIS/COFINS não encontrados.")]
        public IHttpActionResult ObterCstsPisCofins()
        {
            using (var sessao = new GDATransaction())
            {
                var cstsPisCofins = Data.EFD.DataSourcesEFD.Instance.GetCstPisCofins(true)
                    .Select(c => new IdNomeDto()
                    {
                        Id = (int?)c.Id,
                        Nome = c.Descr,
                    });

                return this.Lista(cstsPisCofins);
            }
        }
    }
}
