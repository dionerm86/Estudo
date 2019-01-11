// <copyright file="GetExportacaoPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Exportacao.Lista.V1;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Exportacao.V1
{
    /// <summary>
    /// Controller de exportacao de pedidos.
    /// </summary>
    public partial class ExportacaoPedidosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de exportação de pedidos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das exportações de pedidos.</param>
        /// <returns>Uma lista JSON com os dados das exportações de pedidos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Exportações de pedidos encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Exportações de pedidos não encontradas.")]
        [SwaggerResponse(206, "Exportações de pedidos paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaExportacaoPedidos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var exportacaoPedidos = ExportacaoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (int)filtro.Situacao,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    exportacaoPedidos.Select(ep => new ListaDto(ep)),
                    filtro,
                    () => ExportacaoDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (int)filtro.Situacao,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString()));
            }
        }

        /// <summary>
        /// Consultar Situações da Exportação de Pedidos.
        /// </summary>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ConsultaSituacoesExportacaoPedidos()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoesExportacaoPedido = new ConversorEnum<PedidoExportacao.SituacaoExportacaoEnum>()
                    .ObterTraducao();

                return this.Item(situacoesExportacaoPedido);
            }
        }
    }
}
