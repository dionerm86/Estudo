// <copyright file="PostPedidosConferenciaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
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
        /// Reabre um pedido em conferência.
        /// </summary>
        /// <param name="id">O identificador do pedido em conferência.</param>
        /// <returns>Um status HTTP indicando se o pedido em conferência foi reaberto.</returns>
        [HttpPost]
        [Route("{id}/reabrir")]
        [SwaggerResponse(200, "Pedido em conferência reaberto.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na reabertura do pedido em conferência.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido em conferência não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult Reabrir(int id)
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

                    PedidoEspelhoDAO.Instance.ReabrirPedido(sessao, (uint)id, false);

                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao reabrir pedido em conferência.", ex);
                }
            }
        }

        /// <summary>
        /// Altera a situação CNC do pedido em conferência.
        /// </summary>
        /// <param name="id">O identificador do pedido em conferência.</param>
        /// <returns>Um status HTTP indicando se a situação CNC do pedido em conferência foi alteraada.</returns>
        [HttpPost]
        [Route("{id}/alterarSituacaoCnc")]
        [SwaggerResponse(200, "Situação CNC alterada.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na alteração da situação CNC do pedido em conferência.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido em conferência não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacaoCnc(int id)
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

                    var pedidoConferencia = PedidoEspelhoDAO.Instance.GetElement(sessao, (uint)id);

                    if (pedidoConferencia.ExibirSituacaoCnc)
                    {
                        pedidoConferencia.SituacaoCnc = pedidoConferencia.SituacaoCnc == (int)PedidoEspelho.SituacaoCncEnum.NaoProjetado
                            ? (int)PedidoEspelho.SituacaoCncEnum.Projetado
                            : (int)PedidoEspelho.SituacaoCncEnum.NaoProjetado;
                    }
                    else if (pedidoConferencia.ExibirSituacaoCncConferencia)
                    {
                        pedidoConferencia.SituacaoCnc = pedidoConferencia.SituacaoCnc == (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido
                            ? (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeConferido
                            : (int)PedidoEspelho.SituacaoCncEnum.SemNecessidadeNaoConferido;
                    }

                    pedidoConferencia.DataProjetoCnc = DateTime.Now;
                    pedidoConferencia.UsuProjetoCnc = UserInfo.GetUserInfo.CodUser;

                    LogAlteracaoDAO.Instance.LogPedidoEspelho(sessao, pedidoConferencia, LogAlteracaoDAO.SequenciaObjeto.Novo);
                    PedidoEspelhoDAO.Instance.Update(sessao, pedidoConferencia);

                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao alterar situação CNC.", ex);
                }
            }
        }

        /// <summary>
        /// Marcar pedido importado conferido.
        /// </summary>
        /// <param name="id">O identificador do pedido em conferência.</param>
        /// <returns>Um status HTTP indicando se o pedido em conferência importado foi marcado como conferido.</returns>
        [HttpPost]
        [Route("{id}/marcarPedidoImportadoConferido")]
        [SwaggerResponse(200, "Pedido importado marcado como conferido.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na marcação de pedido importado como conferido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido em conferência não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult MarcarPedidoImportadoConferido(int id)
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

                    PedidoEspelhoDAO.Instance.AlteraSituacaoPedidoImportadoConferido(sessao, id);

                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao marcar pedido importado como conferido.", ex);
                }
            }
        }
    }
}
