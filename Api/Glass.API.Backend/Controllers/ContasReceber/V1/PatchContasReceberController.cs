// <copyright file="PatchContasReceberController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.ContasReceber;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasReceber.V1.CadastroAtualizacao;
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
        /// Atualiza dados da conta recebida.
        /// </summary>
        /// <param name="id">O identificador da conta recebida que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na conta recebida indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/recebida")]
        [SwaggerResponse(202, "Dados da conta recebida alterados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Conta recebida não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarContaRecebida(int id, [FromBody] CadastroAtualizacaoRecebidaDto dadosParaAlteracao)
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

                    var contaRecebida = ContasReceberDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (contaRecebida == null)
                    {
                        return this.NaoEncontrado($"Conta recebida {id} não encontrada.");
                    }

                    contaRecebida = new ConverterCadastroAtualizacaoParaContaRecebida(dadosParaAlteracao, contaRecebida)
                        .ConverterParaContaRecebida();

                    sessao.BeginTransaction();

                    ContasReceberDAO.Instance.Update(sessao, contaRecebida);
                    sessao.Commit();

                    return this.Aceito($"Conta recebida {id} atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar conta recebida.", ex);
                }
            }
        }
    }
}
