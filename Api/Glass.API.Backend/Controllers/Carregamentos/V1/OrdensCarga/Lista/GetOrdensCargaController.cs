// <copyright file="GetOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using WebGlass.Business.OrdemCarga.Fluxo;
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

                var situacoes = filtro.SituacoesOrdemCarga != null && filtro.SituacoesOrdemCarga.Any() ? string.Join(",", filtro.SituacoesOrdemCarga.Select(soc => (int)soc).ToArray()) : null;
                var tipos = filtro.TiposOrdemCarga != null && filtro.TiposOrdemCarga.Any() ? string.Join(",", filtro.TiposOrdemCarga.Select(toc => (int)toc).ToArray()) : null;
                var rotasExternas = filtro.IdsRotaExterna != null && filtro.IdsRotaExterna.Any() ? string.Join(",", filtro.IdsRotaExterna.ToArray()) : null;

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
                    situacoes,
                    tipos,
                    (uint)(filtro.IdClienteExterno ?? 0),
                    filtro.NomeClienteExterno,
                    string.Join(",", rotasExternas),
                    null,
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
                        situacoes,
                        tipos,
                        (uint)(filtro.IdClienteExterno ?? 0),
                        filtro.NomeClienteExterno,
                        rotasExternas));
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
        [SwaggerResponse(204, "Tipos de ordem de carga não encontrados.")]
        public IHttpActionResult ObterTiposOrdemCarga()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new Helper.ConversorEnum<Models.Carregamentos.V1.OrdensCarga.Lista.TiposOrdemCarga.TipoMovimentacao>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera as situações de ordem de carga.
        /// </summary>
        /// <returns>Uma lista JSON com as situações de ordem de carga.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de ordem de carga encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(200, "Situações de ordem de carga não encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterSituacoesOrdemCarga()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new Helper.ConversorEnum<Models.Carregamentos.V1.OrdensCarga.Lista.SituacoesOrdemCarga.SituacoesOrdemCarga>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de pedidos.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga utilizado na busca.</param>
        /// <returns>Uma lista JSON com os dados dos pedidos.</returns>
        [HttpGet]
        [Route("{id:int}/pedidos")]
        [SwaggerResponse(200, "Pedidos encontrados.", Type = typeof(IEnumerable<Models.Pedidos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Pedidos não encontrados para o filtro informado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPedidosParaOrdemCarga(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                var idsPedidos = string.Join(",", Data.DAL.PedidoDAO.Instance.GetIdsPedidosForOC((uint)id));

                var pedidos = Data.DAL.PedidoDAO.Instance.GetPedidosForOC(idsPedidos, (uint)id, false);

                return this.Lista(pedidos.Select(p => new Models.Carregamentos.V1.OrdensCarga.Lista.Pedidos.ListaDto(p)));
            }
        }

        /// <summary>
        /// Recupera a permissão para associar pedidos a ordem de carga.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga.</param>
        /// <returns>Um objeto JSON que define a permissão para associar pedidos.</returns>
        [HttpGet]
        [Route("{id:int}/verificarPermissaoAssociarPedidos")]
        [SwaggerResponse(200, "Associação de pedidos autorizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPermissaoAssociarPedidos(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var podeAssociarPedidosOrdemCarga = OrdemCargaFluxo.Instance.PodeAdicionarPedido((uint)id);

                    if (!podeAssociarPedidosOrdemCarga)
                    {
                        return this.ErroValidacao($"Associação de pedidos a ordem de carga não autorizada.");
                    }

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    return this.ErroValidacao($"Erro ao obter permissão para associar pedidos a ordem de carga.", ex);
                }
            }
        }
    }
}