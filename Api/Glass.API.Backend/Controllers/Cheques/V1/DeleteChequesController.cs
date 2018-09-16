// <copyright file="DeleteChequesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1
{
    /// <summary>
    /// Controller de cheques.
    /// </summary>
    public partial class ChequesController : BaseController
    {
        /// <summary>
        /// Cancela a devolução do cheque.
        /// </summary>
        /// <param name="id">O identificador do cheque que terá a devolução cancelada.</param>
        /// <returns>Um status HTTP indicando se a devolução do cheque foi cancelada.</returns>
        [HttpDelete]
        [Route("{id}/devolucao")]
        [SwaggerResponse(202, "Devolução de cheque cancelada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cheque não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarDevolucao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCheque(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    DepositoChequeDAO.Instance.CancelarDevolucao(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("Devolução de cheque cancelada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao cancelar devolução.", ex);
                }
            }
        }

        /// <summary>
        /// Cancela o protesto do cheque.
        /// </summary>
        /// <param name="id">O identificador do cheque que terá o protesto cancelado.</param>
        /// <returns>Um status HTTP indicando se a protesto do cheque foi cancelado.</returns>
        [HttpDelete]
        [Route("{id}/protesto")]
        [SwaggerResponse(202, "Protesto de cheque cancelado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cheque não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarProtesto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCheque(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    Data.Model.Cheques cheque = ChequesDAO.Instance.GetElement(sessao, (uint)id);
                    cheque.Situacao = (int)Data.Model.Cheques.SituacaoCheque.Devolvido;

                    ChequesDAO.Instance.UpdateBase(sessao, cheque, false);

                    sessao.Commit();

                    return this.Aceito("Protesto de cheque cancelado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao cancelar protesto.", ex);
                }
            }
        }

        /// <summary>
        /// Cancela a reapresentação do cheque.
        /// </summary>
        /// <param name="id">O identificador do cheque que terá a reapresentação cancelada.</param>
        /// <returns>Um status HTTP indicando se a reapresentação do cheque foi cancelada.</returns>
        [HttpDelete]
        [Route("{id}/reapresentacao")]
        [SwaggerResponse(202, "Reapresentação de cheque cancelado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cheque não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarReapresentacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCheque(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ChequesDAO.Instance.CancelarReapresentacaoDeCheque(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("Reapresentação de cheque cancelada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao cancelar reapresentação.", ex);
                }
            }
        }
    }
}
