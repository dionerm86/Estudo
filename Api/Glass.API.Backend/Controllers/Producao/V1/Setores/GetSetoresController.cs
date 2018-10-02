// <copyright file="GetSetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores de produção.
    /// </summary>
    public partial class SetoresController : BaseController
    {
        /// <summary>
        /// Recupera uma lista com os setores de produção para uso no controle.
        /// </summary>
        /// <param name="incluirSetorImpressao">Indica se o setor de impressão de etiquetas deve ser retornado.</param>
        /// <param name="incluirEtiquetaNaoImpressa">Indica se deve ser retornado um setor 'Etiqueta não impressa'.</param>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Setores de produção encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Setores de produção não encontrados.")]
        public IHttpActionResult ObterParaControle(bool incluirSetorImpressao, bool incluirEtiquetaNaoImpressa)
        {
            using (var sessao = new GDATransaction())
            {
                var setores = Utils.GetSetores
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdSetor,
                        Nome = s.Descricao,
                    })
                    .ToList();

                if (!incluirSetorImpressao)
                {
                    setores = setores
                        .Where(s => s.Id != 1)
                        .ToList();
                }

                if (incluirEtiquetaNaoImpressa)
                {
                    setores.Insert(0, new IdNomeDto
                    {
                        Id = -1,
                        Nome = "Etiqueta não impressa",
                    });
                }

                return this.Lista(setores);
            }
        }
    }
}
