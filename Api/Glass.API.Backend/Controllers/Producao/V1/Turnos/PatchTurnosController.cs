// <copyright file="PatchTurnosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Producao.Turnos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Producao.V1.Turnos.CadastroAtualizacao;
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
        /// Atualiza um turno.
        /// </summary>
        /// <param name="id">O identificador do turno que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no turno indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Turno alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Turno não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarTurno(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
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

                    var turnoAtual = fluxo.ObtemTurno(id);

                    turnoAtual = new ConverterCadastroAtualizacaoParaTurno(dadosParaAlteracao, turnoAtual)
                        .ConverterParaTurno();

                    var resultado = fluxo.SalvarTurno(turnoAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar turno. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Turno atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar turno.", ex);
                }
            }
        }
    }
}
