// <copyright file="GetGruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    public partial class GruposProdutoController : BaseController
    {
        /// <summary>
        /// Recupera os grupos de produto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os grupos de produto encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Grupos de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Grupos de produto não encontrados.")]
        public IHttpActionResult ObterGruposProdutoParaFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = GrupoProdDAO.Instance.GetForFilter()
                    .Select(g => new IdNomeDto
                    {
                        Id = g.IdGrupoProd,
                        Nome = g.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
