// <copyright file="AmbientesPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.AmbientesPedido
{
    /// <summary>
    /// Controller de ambientes de pedido.
    /// </summary>
    [RoutePrefix("api/v1/pedidos/{idPedido}/ambientes")]
    public partial class AmbientesPedidoController : BaseController
    {
        private IHttpActionResult ValidarIdPedido(GDASession sessao, int idPedido)
        {
            if (idPedido <= 0)
            {
                return this.ErroValidacao("Número do pedido deve ser maior que zero.");
            }

            if (!PedidoDAO.Instance.Exists(sessao, idPedido))
            {
                return this.NaoEncontrado(string.Format("Pedido {0} não encontrado.", idPedido));
            }

            return null;
        }

        private IHttpActionResult ValidarOperacaoId(GDASession sessao, int idPedido, int id, out AmbientePedido ambiente)
        {
            ambiente = null;

            var validacao = this.ValidarIdPedido(sessao, idPedido);

            if (validacao != null)
            {
                return validacao;
            }

            if (id <= 0)
            {
                return this.ErroValidacao("Número do ambiente deve ser maior que zero.");
            }

            ambiente = AmbientePedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);

            if (ambiente == null)
            {
                return this.NaoEncontrado(string.Format("Ambiente de pedido {0} não encontrado.", id));
            }

            if (ambiente.IdPedido != idPedido)
            {
                return this.NaoEncontrado(string.Format("Ambiente de pedido {0} não pertence ao pedido {1}.", id, idPedido));
            }

            return null;
        }
    }
}
