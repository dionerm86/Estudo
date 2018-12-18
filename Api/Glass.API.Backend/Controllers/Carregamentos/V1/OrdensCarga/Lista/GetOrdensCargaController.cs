// <copyright file="GetOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de ordens de carga.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das ordens de carga.</param>
        /// <returns>Uma lista JSON com os dados das ordens de carga.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Ordens de carga encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Carregamentos.V1.OrdensCarga.Lista.ListaDto>))]
        [SwaggerResponse(204, "Ordens de carga não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Ordens de carga paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Carregamentos.V1.OrdensCarga.Lista.ListaDto>))]
        public IHttpActionResult ObterListaOrdensCarga([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var ordensCarga = OrdemCargaDAO.Instance.GetListWithExpression(
                    (uint)(filtro.IdCarregamento ?? 0),
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdRota ?? 0),
                    filtro.PeriodoEntregaPedidoInicio?.ToShortDateString(),
                    filtro.PeriodoEntregaPedidoFim?.ToShortDateString(),
                    string.Join(",", filtro.SituacoesOrdemCarga.ToString()),
                    string.Join(",", filtro.TiposOrdemCarga.ToString()),
                    (uint)(filtro.IdClienteExterno ?? 0),
                    filtro.NomeClienteExterno,
                    string.Join(",", filtro.IdsRotaExterna.ToString()),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    ordensCarga.Select(o => new Models.Carregamentos.V1.OrdensCarga.Lista.ListaDto(o)),
                    filtro,
                    () => OrdemCargaDAO.Instance.GetListWithExpressionCount(
                        (uint)(filtro.IdCarregamento ?? 0),
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdRota ?? 0),
                        filtro.PeriodoEntregaPedidoInicio?.ToShortDateString(),
                        filtro.PeriodoEntregaPedidoFim?.ToShortDateString(),
                        string.Join(",", filtro.SituacoesOrdemCarga.ToString()),
                        string.Join(",", filtro.TiposOrdemCarga.ToString()),
                        (uint)(filtro.IdClienteExterno ?? 0),
                        filtro.NomeClienteExterno,
                        string.Join(",", filtro.IdsRotaExterna.ToString())));
            }
        }

        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de ordens de carga.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Carregamentos.V1.OrdensCarga.Lista.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaOrdensCarga()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Carregamentos.V1.OrdensCarga.Lista.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera os tipos de ordem de carga.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos de ordem de carga.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de ordem de carga encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterTiposOrdemCarga()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new Helper.ConversorEnum<Models.Carregamentos.V1.OrdensCarga.Lista.TiposOrdemCarga.TipoMovimentação>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}