// <copyright file="DeleteGruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    public partial class GruposContaController : BaseController
    {
        /// <summary>
        /// Exclui um grupo de conta.
        /// </summary>
        /// <param name="id">O identificador do grupo de conta que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de conta excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de conta não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirGrupoConta(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdGrupoConta(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>();

                    var grupoConta = fluxo.ObtemGrupoConta(id);

                    var resultado = fluxo.ApagarGrupoConta(grupoConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir grupo de conta. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Grupo de conta excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir grupo de conta.", ex);
                }
            }
        }
    }
}
