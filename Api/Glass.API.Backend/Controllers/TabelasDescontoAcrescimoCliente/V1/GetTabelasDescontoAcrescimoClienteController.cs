// <copyright file="GetTabelasDescontoAcrescimoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TabelasDescontoAcrescimoCliente.V1
{
    /// <summary>
    /// Controller de tabelas de desconto/acréscimo de cliente.
    /// </summary>
    public partial class TabelasDescontoAcrescimoClienteController : BaseController
    {
        /// <summary>
        /// Recupera as tabelas de desconto/acréscimo para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as tabelas de desconto/acréscimo encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Tabelas de desconto/acréscimo encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tabelas de desconto/acréscimo não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var tabelas = TabelaDescontoAcrescimoClienteDAO.Instance.GetSorted()
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdTabelaDesconto,
                        Nome = p.Descricao,
                    });

                return this.Lista(tabelas);
            }
        }
    }
}
