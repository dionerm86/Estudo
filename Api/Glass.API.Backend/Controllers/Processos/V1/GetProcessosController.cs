// <copyright file="GetProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Processos.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Processos.V1
{
    /// <summary>
    /// Controller de processos (etiqueta).
    /// </summary>
    public partial class ProcessosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de processos (etiqueta) para controle de filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos processos de etiqueta.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Processos encontrados.", Type = typeof(IEnumerable<ProcessoDto>))]
        [SwaggerResponse(204, "Processos não encontrados.")]
        public IHttpActionResult ObterParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var processos = EtiquetaProcessoDAO.Instance.GetForFilter()
                    .Select(p => new ProcessoDto
                    {
                        Id = p.IdProcesso,
                        Codigo = p.CodInterno,
                        Aplicacao = this.ObterAplicacao(p.IdAplicacao, p.CodAplicacao),
                    });

                return this.Lista(processos);
            }
        }

        private IdCodigoDto ObterAplicacao(int? id, string codigo)
        {
            return !id.HasValue || string.IsNullOrWhiteSpace(codigo)
                ? null
                : new IdCodigoDto
                {
                    Id = id.Value,
                    Codigo = codigo,
                };
        }
    }
}
