// <copyright file="DeletePedidosConferenciaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PedidosConferencia.V1
{
    /// <summary>
    /// Controller de pedidos em conferência.
    /// </summary>
    public partial class PedidosConferenciaController : BaseController
    {
        /// <summary>
        /// Exclui um pedido em conferência, se possível.
        /// </summary>
        /// <param name="id">O identificador do pedido em conferência que será excluído.</param>
        /// <returns>Um status HTTP indicando se o pedido em conferência foi excluído.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(200, "Pedido em conferência cancelado.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação no cancelamento do pedido em conferência.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido em conferência não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult Excluir(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedidoConferencia(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    PedidoEspelhoDAO.Instance.CancelarEspelho(sessao, (uint)id);
                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao cancelar a conferência.", e);
                }
            }
        }
    }
}
