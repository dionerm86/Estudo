// <copyright file="DeleteAmbientesPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.AmbientesPedido
{
    /// <summary>
    /// Controller de ambientes de pedido.
    /// </summary>
    public partial class AmbientesPedidoController : BaseController
    {
        /// <summary>
        /// Exclui um ambiente do pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="id">O identificador do ambiente.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Ambiente excluído do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido ou ambiente não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult Excluir(int idPedido, int id)
        {
            using (var sessao = new GDATransaction())
            {
                AmbientePedido ambiente;
                var validacao = this.ValidarOperacaoId(sessao, idPedido, id, out ambiente);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    AmbientePedidoDAO.Instance.Delete(sessao, ambiente);
                    sessao.Commit();

                    return this.Aceito(string.Format("Ambiente {0} excluído com sucesso do pedido {1}!", id, idPedido));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao excluir o ambiente {0} do pedido {1}.", id, idPedido), e);
                }
            }
        }
    }
}
