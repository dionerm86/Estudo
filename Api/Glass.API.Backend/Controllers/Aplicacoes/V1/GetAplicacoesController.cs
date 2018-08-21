// <copyright file="GetAplicacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Aplicacoes.V1
{
    /// <summary>
    /// Controller de aplicações (etiqueta).
    /// </summary>
    public partial class AplicacoesController : BaseController
    {
        /// <summary>
        /// Recupera a lista de aplicações (etiqueta) para controle de filtro.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das aplicações de etiqueta.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Aplicações encontradas.", Type = typeof(IEnumerable<IdCodigoDto>))]
        [SwaggerResponse(204, "Aplicações não encontradas.")]
        public IHttpActionResult ObterParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var processos = EtiquetaAplicacaoDAO.Instance.GetForFilter()
                    .Select(p => new IdCodigoDto
                    {
                        Id = p.IdAplicacao,
                        Codigo = p.CodInterno,
                    });

                return this.Lista(processos);
            }
        }
    }
}
