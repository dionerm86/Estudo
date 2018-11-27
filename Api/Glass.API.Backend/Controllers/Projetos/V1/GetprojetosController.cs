// <copyright file="GetProjetosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Projetos.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1
{
    /// <summary>
    /// Controller de projetos.
    /// </summary>
    public partial class ProjetosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de projetos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Projetos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Projetos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Projetos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterProjetos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var pedidos = ProjetoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                    filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pedidos.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProjetoDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                        filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null));
            }
        }
    }
}
