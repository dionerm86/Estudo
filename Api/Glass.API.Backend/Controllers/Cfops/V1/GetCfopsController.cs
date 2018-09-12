// <copyright file="GetCfopsController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1
{
    /// <summary>
    /// Controller de cfops.
    /// </summary>
    public partial class CfopsController : BaseController
    {
        /// <summary>
        /// Recupera os cfops para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos cfops encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "CFOPs encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CFOPs não encontrados.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var cfops = CfopDAO.Instance.GetSortedByCodInterno()
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdCfop,
                        Nome = c.CodInterno,
                    });

                return this.Lista(cfops);
            }
        }

        /// <summary>
        /// Recupera os tipos de cfop para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos tipos de cfop encontrados.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de CFOP encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de CFOP não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposCfop = TipoCfopDAO.Instance.GetAll()
                    .Select(c => new IdNomeDto()
                    {
                        Id = c.IdTipoCfop,
                        Nome = c.Descricao,
                    });

                return this.Lista(tiposCfop);
            }
        }
    }
}
