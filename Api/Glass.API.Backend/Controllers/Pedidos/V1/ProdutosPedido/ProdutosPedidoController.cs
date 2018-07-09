// <copyright file="ProdutosPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Configuracoes;
using Glass.Data.DAL;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.ProdutosPedido
{
    /// <summary>
    /// Controller de produtos de pedido.
    /// </summary>
    [RoutePrefix("api/v1/pedidos/{idPedido}/produtos")]
    public partial class ProdutosPedidoController : BaseController
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

        private IHttpActionResult ValidarIdsPedidoEAmbiente(GDASession sessao, int idPedido, int? idAmbiente)
        {
            var validacao = this.ValidarIdPedido(sessao, idPedido);

            if (validacao != null)
            {
                return validacao;
            }

            if (PedidoConfig.DadosPedido.AmbientePedido)
            {
                if (!idAmbiente.HasValue)
                {
                    return this.ErroValidacao("O ambiente de pedido é obrigatório.");
                }

                if (idAmbiente.Value <= 0)
                {
                    return this.ErroValidacao("Número do ambiente de pedido deve ser maior que zero.");
                }

                if (!AmbientePedidoDAO.Instance.Exists(sessao, idAmbiente.Value))
                {
                    return this.NaoEncontrado($"Ambiente de pedido {idAmbiente} não encontrado.");
                }

                var idsAmbientesPedido = AmbientePedidoDAO.Instance.GetIdsByPedido(sessao, (uint)idPedido);

                if (!idsAmbientesPedido.Contains((uint)idAmbiente.Value))
                {
                    return this.NaoEncontrado($"O ambiente de pedido {idAmbiente} não pertence ao pedido {idPedido}.");
                }
            }

            return null;
        }

        private IHttpActionResult ValidarOperacaoId(GDASession sessao, int idPedido, int id, out Data.Model.ProdutosPedido produtoPedido)
        {
            produtoPedido = null;

            var validacao = this.ValidarIdPedido(sessao, idPedido);

            if (validacao != null)
            {
                return validacao;
            }

            if (id <= 0)
            {
                return this.ErroValidacao("Número do produto deve ser maior que zero.");
            }

            produtoPedido = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);

            if (produtoPedido == null)
            {
                return this.NaoEncontrado($"Produto de pedido {id} não encontrado.");
            }

            if (produtoPedido.IdPedido != idPedido)
            {
                return this.NaoEncontrado($"Produto de pedido {id} não pertence ao pedido {idPedido}.");
            }

            return null;
        }
    }
}
