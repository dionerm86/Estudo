// <copyright file="GetAcertosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Acertos.V1
{
    /// <summary>
    /// Controller de acertos.
    /// </summary>
    public partial class AcertosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de acertos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Acertos.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaAcertos()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Acertos.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de acertos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Acertos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Acertos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Acertos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Acertos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Acertos.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterAcertos([FromUri] Models.Acertos.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Acertos.V1.Lista.FiltroDto();

                var acertos = AcertoDAO.Instance.GetByCliList(
                    filtro.Id ?? 0,
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdLiberacao ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    (uint)(filtro.IdFormaPagamento ?? 0),
                    0,
                    filtro.NumeroNfe ?? 0,
                    (int)filtro.Protesto,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    acertos.Select(o => new Models.Acertos.V1.Lista.ListaDto(o)),
                    filtro,
                    () => AcertoDAO.Instance.GetByCliListCount(
                        filtro.Id ?? 0,
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdLiberacao ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        (uint)(filtro.IdFormaPagamento ?? 0),
                        0,
                        filtro.NumeroNfe ?? 0,
                        (int)filtro.Protesto));
            }
        }
    }
}
