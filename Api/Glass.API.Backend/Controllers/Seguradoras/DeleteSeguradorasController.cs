// <copyright file="DeleteSeguradorasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Seguradoras.V1
{
    /// <summary>
    /// Controller de seguradoras.
    /// </summary>
    public partial class SeguradorasController : BaseController
    {
        /// <summary>
        /// Exclui uma seguradora.
        /// </summary>
        /// <param name="id">O identificador da seguradora que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Seguradora excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Seguradora não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirSeguradora(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSeguradora(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICTeFluxo>();

                    var seguradora = fluxo.ObtemSeguradora(id);

                    var resultado = fluxo.ApagarSeguradora(seguradora);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir seguradora. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Seguradora excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir seguradora.", ex);
                }
            }
        }
    }
}
