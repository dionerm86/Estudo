// <copyright file="PostLiberacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Liberacoes.V1
{
    /// <summary>
    /// Controller de liberações.
    /// </summary>
    public partial class LiberacoesController : BaseController
    {
        /// <summary>
        /// Reenvia o email da liberação.
        /// </summary>
        /// <param name="id">O identificador da liberação que será reenviado o email.</param>
        /// <returns>Um status HTTP indicando se o email foi inserido na fila de envio.</returns>
        [HttpPost]
        [Route("{id}/reenviarEmail")]
        [SwaggerResponse(202, "Email adicionado na fila de envio.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação no reenvio de email de liberação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Liberação não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ReenviarEmail(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdLiberacao(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    Email.EnviaEmailLiberacao(sessao, (uint)id);

                    LogAlteracaoDAO.Instance.LogReenvioEmailLiberacao(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("O e-mail foi adicionado na fila para ser enviado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }
    }
}
