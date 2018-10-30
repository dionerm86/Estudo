// <copyright file="DeleteAplicacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Aplicacoes.V1
{
    /// <summary>
    /// Controller de aplicações (etiqueta).
    /// </summary>
    public partial class AplicacoesController : BaseController
    {
        /// <summary>
        /// Exclui uma aplicação de etiqueta.
        /// </summary>
        /// <param name="id">O identificador da aplicação que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Aplicação excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Aplicação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirAplicacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdAplicacao(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    sessao.BeginTransaction();

                    EtiquetaAplicacaoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Aplicação de etiqueta {id} excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir aplicação {id}.", ex);
                }
            }
        }
    }
}
