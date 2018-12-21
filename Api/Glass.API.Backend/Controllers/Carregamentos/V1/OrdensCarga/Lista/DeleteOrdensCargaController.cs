// <copyright file="DeleteOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Controller de ordem de carga.
    /// </summary>
    public partial class OrdensCargaController : BaseController
    {
        /// <summary>
        /// Exclui uma ordem de carga.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Ordem de carga excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirOrdemCarga(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var ordemCarga = OrdemCargaDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.Delete(sessao, ordemCarga);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito($"Ordem de carga excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir ordem de carga.", ex);
                }
            }
        }

        /// <summary>
        /// Desassocia um pedido de uma ordem de carga.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga.</param>
        /// <param name="idPedido">O identificador do pedido que será desassociado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}/desassociarPedido/{idPedido:int}")]
        [SwaggerResponse(202, "Pedido desassociado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult DesassociarPedidoOrdemCarga(int id, int idPedido)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarDesassociacaoPedidoOrdemCarga(sessao, id, idPedido);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    WebGlass.Business.OrdemCarga.Fluxo.PedidosOCFluxo.Instance.RemoverPedido((uint)id, (uint)idPedido);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito($"Pedido {idPedido} desassociado da ordem de carga {id}.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao($"Erro ao desassociar pedido da ordem de carga.", ex);
                }
            }
        }
    }
}