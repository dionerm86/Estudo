// <copyright file="GetGruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Projetos.V1.GruposProjeto.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.GruposProjeto
{
    /// <summary>
    /// Controller de grupos de projeto.
    /// </summary>
    public partial class GruposProjetoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de grupos de projeto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos grupos de projeto.</param>
        /// <returns>Uma lista JSON com os dados dos grupos de projeto.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Grupos de projeto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Grupos de projeto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Grupos de projeto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaGruposProjeto([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var gruposProjeto = GrupoModeloDAO.Instance.GetList(
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    gruposProjeto.Select(g => new ListaDto(g)),
                    filtro,
                    () => GrupoModeloDAO.Instance.GetCount());
            }
        }

        /// <summary>
        /// Recupera as situações de grupo de projeto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Situações de grupo de projeto encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de grupo de projeto não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var grupos = GrupoModeloDAO.Instance.GetOrdered()
                    .Select(f => new IdNomeDto
                    {
                        Id = (int)f.IdGrupoModelo,
                        Nome = f.Descricao,
                    });

                return this.Lista(grupos);
            }
        }
    }
}
