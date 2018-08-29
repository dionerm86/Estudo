// <copyright file="GetFormasPagamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Parcelas.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.FormasPagamento.V1
{
    /// <summary>
    /// Controller de formas de pagamento.
    /// </summary>
    public partial class FormasPagamentoController : BaseController
    {
        /// <summary>
        /// Recupera as formas de pagamento para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as formas de pagamento encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Formas de pagamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagamento = FormaPagtoDAO.Instance.GetForControle()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdFormaPagto,
                        Nome = p.Descricao,
                    });

                return this.Lista(formasPagamento);
            }
        }

        /// <summary>
        /// Recupera as formas de pagamento para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as formas de pagamento encontradas.</returns>
        [HttpGet]
        [Route("filtroNotaFiscal")]
        [SwaggerResponse(200, "Formas de pagamento de nota fiscal encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento de nota fiscal não encontradas.")]
        public IHttpActionResult ObterFiltroNotaFiscal()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagamento = FormaPagtoDAO.Instance.GetForNotaFiscal(0)
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdFormaPagto,
                        Nome = p.Descricao,
                    });

                return this.Lista(formasPagamento);
            }
        }
    }
}
