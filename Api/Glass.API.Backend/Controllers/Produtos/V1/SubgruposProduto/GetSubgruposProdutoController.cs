// <copyright file="GetSubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.SubgruposProduto
{
    /// <summary>
    /// Controller de subgrupos de produto.
    /// </summary>
    public partial class SubgruposProdutoController : BaseController
    {
        /// <summary>
        /// Recupera os subgrupos de produto para os controles de filtro das telas.
        /// </summary>
        /// <param name="idGrupoProduto">O identificador do grupo de produto.</param>
        /// <returns>Uma lista JSON com os subgrupos de produto encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Subgrupos de produto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Subgrupos de produto não encontrados.")]
        public IHttpActionResult ObterSubgruposProdutoParaFiltro(int? idGrupoProduto = null)
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = SubgrupoProdDAO.Instance.GetForFilter(idGrupoProduto.GetValueOrDefault())
                    .Where(g => g.IdSubgrupoProd > 0)
                    .Select(g => new IdNomeDto
                    {
                        Id = g.IdSubgrupoProd,
                        Nome = g.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
