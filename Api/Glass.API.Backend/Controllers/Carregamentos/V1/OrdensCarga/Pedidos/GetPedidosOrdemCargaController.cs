// <copyright file="GetPedidosOrdemCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Pedidos
{
    /// <summary>
    /// Controller de pedidos associados a uma ordem de carga.
    /// </summary>
    public partial class PedidosOrdemCargaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de pedidos.
        /// </summary>
        /// <param name="idOrdemCarga">O identificador da ordem de carga utilizado na busca.</param>
        /// <returns>Uma lista JSON com os dados dos pedidos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pedidos encontrados.", Type = typeof(IEnumerable<Models.Pedidos.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Pedidos não encontrados para o filtro informado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPedidosParaOrdemCarga(int idOrdemCarga)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, idOrdemCarga);

                if (validacao != null)
                {
                    return validacao;
                }

                var idsPedidos = string.Join(",", Data.DAL.PedidoDAO.Instance.GetIdsPedidosForOC((uint)idOrdemCarga));

                var pedidos = Data.DAL.PedidoDAO.Instance.GetPedidosForOC(idsPedidos, (uint)idOrdemCarga, false);

                return this.Lista(pedidos.Select(p => new Models.Carregamentos.V1.OrdensCarga.Lista.Pedidos.ListaDto(p)));
            }
        }
    }
}