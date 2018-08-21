// <copyright file="GetProdutosPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.ProdutosPedido.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.ProdutosPedido
{
    /// <summary>
    /// Controller de produtos de pedido.
    /// </summary>
    public partial class ProdutosPedidoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de produtos de um pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="filtro">Filtro usado para a busca dos produtos do pedido.</param>
        /// <returns>Um objeto JSON com a lista de produtos encontrados para o pedido.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Produtos do pedido sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Produtos do pedido não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Produtos do pedido paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo idPedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterLista(int idPedido, [FromUri] FiltroListaDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroListaDto();
                var validacao = this.ValidarIdsPedidoEAmbiente(sessao, idPedido, filtro.IdAmbiente);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var produtos = ProdutosPedidoDAO.Instance.GetList(
                        (uint)idPedido,
                        (uint)filtro.IdAmbiente.GetValueOrDefault(),
                        filtro.IdProdutoPai > 0,
                        (uint)filtro.IdProdutoPai.GetValueOrDefault(),
                        filtro.ObterTraducaoOrdenacao(),
                        filtro.ObterPrimeiroRegistroRetornar(),
                        filtro.NumeroRegistros);

                    return this.ListaPaginada(
                        produtos.Where(p => p.IdProdPed > 0).Select(p => new ListaDto(p)),
                        filtro,
                        () => ProdutosPedidoDAO.Instance.GetCount(
                            (uint)idPedido,
                            (uint)filtro.IdAmbiente.GetValueOrDefault(),
                            filtro.IdProdutoPai > 0,
                            (uint)filtro.IdProdutoPai.GetValueOrDefault()));
                }
                catch (Exception e)
                {
                    return this.ErroValidacao(string.Format("Erro ao recuperar produtos do pedido {0}.", idPedido), e);
                }
            }
        }
    }
}
