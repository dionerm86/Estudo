// <copyright file="GetPendenciasCarregamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Carregamentos.V1.Itens.Pendencias;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebGlass.Business.OrdemCarga.Fluxo;

namespace Glass.API.Backend.Controllers.Carregamentos.V1
{
    /// <summary>
    /// Controller de pendencias de carregamentos.
    /// </summary>
    public partial class PendenciasCarregamentosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de pendencias de carregamentos.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Carregamentos.V1.Itens.Pendencias.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Carregamentos.V1.Itens.Pendencias.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de pendencias de carregamentos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos carregamentos.</param>
        /// <returns>Uma lista JSON com os dados dos carregamentos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pendências de carregamento sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Carregamentos.V1.Itens.Pendencias.ListaDto>))]
        [SwaggerResponse(204, "Pendências de carregamento não encontradas.")]
        [SwaggerResponse(206, "Pendências de carregamento paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Carregamentos.V1.Itens.Pendencias.ListaDto>))]
        public IHttpActionResult ObterListaPendenciasCarregamentos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var pendenciasCarregamentos = PendenciaCarregamentoFluxo.Instance.GetListagemPendenciaCarregamento(
                    (uint)(filtro.IdCarregamento ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdLoja ?? 0),
                    filtro.PeriodoPrevisaoSaidaInicio?.ToShortDateString(),
                    filtro.PeriodoPrevisaoSaidaFim?.ToShortDateString(),
                    filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                    filtro.IgnorarPedidosVendaTransferencia,
                    (uint)(filtro.IdClienteExterno ?? 0),
                    filtro.NomeClienteExterno,
                    filtro.IdsRotaExterna != null && filtro.IdsRotaExterna.Any() ? string.Join(",", filtro.IdsRotaExterna) : null,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    pendenciasCarregamentos.Select(c => new ListaDto(c)),
                    filtro,
                    () => PendenciaCarregamentoFluxo.Instance.GetListagemPendenciaCarregamentoCount(
                        (uint)(filtro.IdCarregamento ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdLoja ?? 0),
                        filtro.PeriodoPrevisaoSaidaInicio?.ToShortDateString(),
                        filtro.PeriodoPrevisaoSaidaFim?.ToShortDateString(),
                        filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : null,
                        filtro.IgnorarPedidosVendaTransferencia,
                        (uint)(filtro.IdClienteExterno ?? 0),
                        filtro.NomeClienteExterno,
                        filtro.IdsRotaExterna != null && filtro.IdsRotaExterna.Any() ? string.Join(",", filtro.IdsRotaExterna) : null));
            }
        }
    }
}
