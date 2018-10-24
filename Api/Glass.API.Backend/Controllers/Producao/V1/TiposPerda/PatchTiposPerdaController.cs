// <copyright file="PatchTiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Producao.TiposPerda;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Producao.V1.TiposPerda.CadastroAtualizacao;
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
        /// Atualiza um tipo de perda.
        /// </summary>
        /// <param name="id">O identificador do tipo de perda que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no tipo de perda indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Tipo de perda alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de perda não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarTipoPerda(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
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

                    var turnoAtual = fluxo.ObtemTipoPerda(id);

                    turnoAtual = new ConverterCadastroAtualizacaoParaTipoPerda(dadosParaAlteracao, turnoAtual)
                        .ConverterParaTipoPerda();

                    var resultado = fluxo.SalvarTipoPerda(turnoAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar tipo de perda. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Tipo de perda atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar tipo de perda.", ex);
                }
            }
        }
    }
}
