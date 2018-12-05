// <copyright file="GetNaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
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
