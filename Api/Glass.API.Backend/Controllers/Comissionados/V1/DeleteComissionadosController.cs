// <copyright file="DeleteComissionadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Comissionados.V1
{
    /// <summary>
    /// Controller de comissionados.
    /// </summary>
    public partial class ComissionadosController : BaseController
    {
        /// <summary>
        /// Exclui um comissionado.
        /// </summary>
        /// <param name="id">O identificador do comissionado que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Comissionado excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Comissionado não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirComissionado(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdComissionado(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IComissionadoFluxo>();

                    var comissionado = fluxo.ObtemComissionado(id);

                    var resultado = fluxo.ApagarComissionado(comissionado);

                    if (!resultado)
                    {
                        return this.ErroValidacao(resultado.Message.Format());
                    }

                    return this.Aceito($"Comissionado excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir comissionado.", ex);
                }
            }
        }
    }
}
