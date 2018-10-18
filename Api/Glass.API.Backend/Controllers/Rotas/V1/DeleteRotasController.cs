// <copyright file="DeleteRotasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Rotas.V1
{
    /// <summary>
    /// Controller de rotas.
    /// </summary>
    public partial class RotasController : BaseController
    {
        /// <summary>
        /// Exclui uma rota.
        /// </summary>
        /// <param name="id">O identificador da rota que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Rota excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Rota não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirRota(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdRota(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IRotaFluxo>();

                    var rota = fluxo.ObtemRota(id);

                    var resultado = fluxo.ApagarRota(rota);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir rota. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Rota excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir rota {id}.", ex);
                }
            }
        }
    }
}
