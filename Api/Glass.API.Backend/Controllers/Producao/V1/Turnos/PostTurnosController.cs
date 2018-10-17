// <copyright file="PostTurnosController.cs" company="Sync Softwares">
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
        /// Cadastra um turno.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um turno.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Turno cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarTurno([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var turno = new ConverterCadastroAtualizacaoParaTurno(dadosParaCadastro)
                        .ConverterParaTurno();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.ITurnoFluxo>()
                        .SalvarTurno(turno);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar turno. {resultado.Message.Format()}");
                    }

                    return this.Criado("Turno cadastrado com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar turno.", ex);
                }
            }
        }
    }
}
