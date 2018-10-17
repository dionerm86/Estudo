// <copyright file="GetAmbientesPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.AmbientesPedido
{
    /// <summary>
    /// Controller de ambientes de pedido.
    /// </summary>
    public partial class AmbientesPedidoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de ambientes de um pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="filtro">Filtro usado para a busca dos ambientes do pedido.</param>
        /// <returns>Um objeto JSON com a lista de ambientes encontrados para o pedido.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Ambientes do pedido sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Ambientes do pedido não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Ambientes do pedido paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo idPedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterLista(int idPedido, [FromUri] FiltroListaDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdPedido(sessao, idPedido);

                if (validacao != null)
                {
                    return validacao;
                }

                filtro = filtro ?? new FiltroListaDto();

                try
                {
                    var ambientes = AmbientePedidoDAO.Instance.GetList(
                        (uint)idPedido,
                        filtro.ObterTraducaoOrdenacao(),
                        filtro.ObterPrimeiroRegistroRetornar(),
                        filtro.NumeroRegistros);

                    return this.ListaPaginada(
                        ambientes.Where(a => a.IdAmbientePedido > 0).Select(a => new ListaDto(a)),
                        filtro,
                        () => AmbientePedidoDAO.Instance.GetCount((uint)idPedido));
                }
                catch (Exception e)
                {
                    return this.ErroValidacao(string.Format("Erro ao recuperar ambientes do pedido {0}.", idPedido), e);
                }
            }
        }
    }
}
