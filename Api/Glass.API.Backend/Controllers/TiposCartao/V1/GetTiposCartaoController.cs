// <copyright file="GetTiposCartaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TiposCartao.V1
{
    /// <summary>
    /// Controller de tipos de cartão.
    /// </summary>
    public partial class TiposCartaoController : BaseController
    {
        /// <summary>
        /// Recupera os tipos de cartão para uso nos controles.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos para os tipos de cartão.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Tipos de cartão encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de cartão não encontrados.")]
        public IHttpActionResult ObterParaControle()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagto = TipoCartaoCreditoDAO.Instance.ObtemListaPorTipo(0, Situacao.Ativo)
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdTipoCartao,
                        Nome = s.Descricao,
                    });

                return this.Lista(formasPagto);
            }
        }
    }
}
