// <copyright file="PatchContasReceberController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasReceber.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasReceber.V1
{
    /// <summary>
    /// Controller de contas a receber/recebidas.
    /// </summary>
    public partial class ContasReceberController : BaseController
    {
        /// <summary>
        /// Atualiza a observação de uma conta recebida.
        /// </summary>
        /// <param name="id">O identificador da conta recebida.</param>
        /// <param name="dadosEntrada">Objeto com a observação da conta recebida.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/alterarObservacoes")]
        [SwaggerResponse(202, "Observação da conta recebida atualizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Conta recebida não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarObservacao(int id, [FromBody] ObservacaoContaRecebidaDto dadosEntrada)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdContaReceber(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var contaReceber = ContasReceberDAO.Instance.GetElementByPrimaryKey(id);
                    contaReceber.Obs = dadosEntrada.Observacao;

                    ContasReceberDAO.Instance.Update(sessao, contaReceber);
                    sessao.Commit();

                    return this.Aceito("Conta recebida atualizada com sucesso.");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar a observação da conta recebida.", e);
                }
            }
        }
    }
}
