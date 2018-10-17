// <copyright file="DeleteTurnosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Turnos
{
    /// <summary>
    /// Controller de turnos.
    /// </summary>
    public partial class TurnosController : BaseController
    {
        /// <summary>
        /// Exclui um turno.
        /// </summary>
        /// <param name="id">O identificador do turno que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Turno excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Turno não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTurno(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTurno(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.ITurnoFluxo>();

                    var turno = fluxo.ObtemTurno(id);

                    var resultado = fluxo.ApagarTurno(turno);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir turno. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Turno excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir turno.", ex);
                }
            }
        }
    }
}
