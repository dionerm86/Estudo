// <copyright file="PedidosConferenciaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PedidosConferencia.V1
{
    /// <summary>
    /// Controller de pedidos em conferência.
    /// </summary>
    [RoutePrefix("api/v1/pedidosConferencia")]
    public partial class PedidosConferenciaController : BaseController
    {
        private IHttpActionResult ValidarIdPedidoConferencia(int id)
        {
            if (id <= 0)
            {
                return this.ErroValidacao("Identificador do pedido em conferência deve ser um número maior que zero.");
            }

            return null;
        }

        private IHttpActionResult ValidarExistenciaIdPedidoConferencia(GDASession sessao, int id)
        {
            var validacao = this.ValidarIdPedidoConferencia(id);

            if (validacao == null && !PedidoEspelhoDAO.Instance.Exists(sessao, id))
            {
                return this.NaoEncontrado("Pedido em conferência não encontrado.");
            }

            return null;
        }
    }
}
