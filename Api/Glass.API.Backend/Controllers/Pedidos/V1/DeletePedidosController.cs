// <copyright file="DeletePedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1
{
    /// <summary>
    /// Controller de pedidos.
    /// </summary>
    public partial class PedidosController : BaseController
    {
        /// <summary>
        /// Exclui um pedido, se possível.
        /// </summary>
        /// <param name="id">O identificador do pedido que será excluído.</param>
        /// <returns>Um status HTTP indicando se o pedido foi excluído.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Pedido excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na exclusão do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirPedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    PedidoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito(string.Format("Pedido {0} excluído com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o pedido.", e);
                }
            }
        }
    }
}
