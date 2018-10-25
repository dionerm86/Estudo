// <copyright file="DeleteContasBancariasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasBancarias.V1
{
    /// <summary>
    /// Controller de contas bancárias.
    /// </summary>
    public partial class ContasBancariasController : BaseController
    {
        /// <summary>
        /// Exclui uma conta bancária.
        /// </summary>
        /// <param name="id">O identificador da conta bancária que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Conta bancária excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Conta bancária não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirContaBancaria(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdContaBancaria(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>();

                    var turno = fluxo.ObtemContaBanco(id);

                    var resultado = fluxo.ApagarContaBanco(turno);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir conta bancária. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Conta bancária excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir conta bancária.", ex);
                }
            }
        }
    }
}
