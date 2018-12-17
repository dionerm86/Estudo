// <copyright file="GetGruposMedidaProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto.GruposMedidaProjeto
{
    /// <summary>
    /// Controller de grupos de medida de projeto.
    /// </summary>
    public partial class GruposMedidaProjetoController : BaseController
    {
        /// <summary>
        /// Recupera a lista dos grupos de medida de projeto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Grupos de medida de projeto encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Grupos de medida de projeto não encontrados.")]
        [SwaggerResponse(206, "Grupos de medida de projeto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterGruposMedidaProjeto([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var gruposMedidaProjeto = GrupoMedidaProjetoDAO.Instance.GetList(
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    gruposMedidaProjeto.Select(g => new ListaDto(g)),
                    filtro,
                    () => GrupoMedidaProjetoDAO.Instance.GetCount());
            }
        }

        /// <summary>
        /// Recupera as situações de grupo de projeto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Grupos de medida de projeto encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Grupos de medida de projeto não encontrados.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var grupos = GrupoMedidaProjetoDAO.Instance.ObtemOrdenado()
                    .Select(f => new IdNomeDto
                    {
                        Id = (int)f.IdGrupoMedProj,
                        Nome = f.Descricao,
                    });

                return this.Lista(grupos);
            }
        }
    }
}
