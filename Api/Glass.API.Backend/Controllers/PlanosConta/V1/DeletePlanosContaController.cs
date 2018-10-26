// <copyright file="DeletePlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um plano de conta.
        /// </summary>
        /// <param name="id">O identificador do plano de conta que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Planos de conta excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Plano de conta não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirPlanoConta(int id)
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

                    var planoConta = fluxo.ObtemPlanoContas(id);

                    var resultado = fluxo.ApagarPlanoContas(planoConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir plano de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Plano de conta excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir plano de conta.", ex);
                }
            }
        }
    }
}
