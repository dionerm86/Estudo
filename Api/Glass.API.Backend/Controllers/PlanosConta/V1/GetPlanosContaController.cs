// <copyright file="GetPlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.PlanosConta.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    public partial class PlanosContaController : BaseController
    {
        /// <summary>
        /// Recupera os planos de conta do sistema para os controles de filtro das telas.
        /// </summary>
        /// <param name="tipo">Tipo do plano de conta (Crédito ou débito)</param>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Planos de conta encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Planos de conta não encontradas.")]
        public IHttpActionResult ObterPlanosConta(Tipo tipo)
        {
            using (var sessao = new GDATransaction())
            {
                var planosConta = PlanoContasDAO.Instance.GetPlanoContas((int)tipo)
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdConta,
                        Nome = p.DescrPlanoGrupo,
                    });

                return this.Lista(planosConta);
            }
        }
    }
}
