// <copyright file="DeleteTiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda
{
    /// <summary>
    /// Controller de tipos de perda.
    /// </summary>
    public partial class TiposPerdaController : BaseController
    {
        /// <summary>
        /// Exclui um tipo de perda.
        /// </summary>
        /// <param name="id">O identificador do tipo de perda que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Tipo de perda excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de perda não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTipoPerda(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTipoPerda(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.IPerdaFluxo>();

                    var tipoPerda = fluxo.ObtemTipoPerda(id);

                    var resultado = fluxo.ApagarTipoPerda(tipoPerda);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir tipo de perda. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Tipo de perda excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir tipo de perda.", ex);
                }
            }
        }
    }
}
