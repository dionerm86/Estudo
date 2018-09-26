// <copyright file="DeleteTransportadoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Transportadores.V1
{
    /// <summary>
    /// Controller de transportadores.
    /// </summary>
    public partial class TransportadoresController : BaseController
    {
        /// <summary>
        /// Exclui um transportador.
        /// </summary>
        /// <param name="id">O identificador do transportador que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Transportador excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Transportador não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTransportador(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTransportador(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.ITransportadorFluxo>();

                    var transportador = fluxo.ObtemTransportador(id);

                    fluxo.ApagarTransportador(transportador);

                    return this.Aceito($"Transportador excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir transportador.", ex);
                }
            }
        }
    }
}
