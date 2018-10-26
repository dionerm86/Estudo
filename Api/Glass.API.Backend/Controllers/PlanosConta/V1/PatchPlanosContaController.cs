// <copyright file="PatchPlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1
{
    /// <summary>
    /// Controller de planos de conta.
    /// </summary>
    public partial class PlanosContaController : BaseController
    {
        /// <summary>
        /// Atualiza um plano de conta.
        /// </summary>
        /// <param name="id">O identificador do plano de conta que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no plano de conta indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Plano de conta alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Plano de conta não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarPlanoConta(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdPlanoConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var planoContaAtual = fluxo.ObtemPlanoContas(id);

                    planoContaAtual = new ConverterCadastroAtualizacaoParaPlanoConta(dadosParaAlteracao, planoContaAtual)
                        .ConverterParaPlanoConta();

                    var resultado = fluxo.SalvarPlanoContas(planoContaAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar plano de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Plano de conta atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar plano de conta.", ex);
                }
            }
        }
    }
}
