// <copyright file="GetPedidosExportacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Pedidos.V1.Exportacao.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.Exportacao
{
    /// <summary>
    /// Controller de pedidos para exportação.
    /// </summary>
    public partial class PedidosExportacaoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de pedidos para exportação.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos pedidos.</param>
        /// <returns>Uma lista JSON com os dados dos pedidos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pedidos para exportação encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Pedidos para exportação não encontrados.")]
        [SwaggerResponse(206, "Pedidos para exportação paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaPedidos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var pedidosParaExportacao = PedidoDAO.Instance.GetForPedidoExportar(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.CodigoPedidoCliente,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString());

                return this.Lista(pedidosParaExportacao.Select(ppe => new ListaDto(ppe)));
            }
        }

        /// <summary>
        /// Recupera as configurações para a lista de pedidos para exportação.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Pedidos.V1.Exportacao.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesLista()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Pedidos.V1.Exportacao.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }
    }
}