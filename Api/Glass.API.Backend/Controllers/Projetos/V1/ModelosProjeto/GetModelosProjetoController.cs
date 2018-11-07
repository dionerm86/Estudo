// <copyright file="GetModelosProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Projetos.V1.ModelosProjeto.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.ModelosProjeto
{
    /// <summary>
    /// Controller de modelos de projeto.
    /// </summary>
    public partial class ModelosProjetoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de modelos de projeto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Modelos de projeto encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Modelos de projeto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Modelos de projeto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterModelosProjeto([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var pedidos = ProjetoModeloDAO.Instance.PesquisarProjetoModelo(
                    filtro.Codigo,
                    filtro.Descricao,
                    (uint)(filtro.IdGrupoModelo ?? 0),
                    (int)(filtro.Situacao ?? 0),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pedidos.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProjetoModeloDAO.Instance.PesquisarProjetoModeloCount(
                        filtro.Codigo,
                        filtro.Descricao,
                        (uint)(filtro.IdGrupoModelo ?? 0),
                        (int)(filtro.Situacao ?? 0)));
            }
        }

        /// <summary>
        /// Recupera as situações de modelo de projeto para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com os itens encontrados.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de modelo de projeto encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de modelo de projeto não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var itens = new ConversorEnum<Data.Model.ProjetoModelo.SituacaoEnum>()
                    .ObterTraducao();

                return this.Lista(itens);
            }
        }
    }
}
