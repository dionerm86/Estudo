// <copyright file="GetProducaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1
{
    /// <summary>
    /// Controller de produção.
    /// </summary>
    public partial class ProducaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de situações de produção possíveis.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de produção não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoProducao()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
