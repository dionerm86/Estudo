// <copyright file="PatchContasPagasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.ContasPagar.Pagas;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasPagar.V1.Pagas.CadastroAtualizacao;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1.Pagas
{
    /// <summary>
    /// Controller de contas pagas.
    /// </summary>
    public partial class ContasPagasController : BaseController
    {
        /// <summary>
        /// Altera uma conta paga.
        /// </summary>
        /// <param name="id"> O identificador da conta paga que será alterada.</param>
        /// <param name="dados"> Os dados utilizados.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Conta paga alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Conta paga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarContaPaga(int id, [FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdContasPagas(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var contaPagaAtual = ContasPagarDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    contaPagaAtual = new ConverterCadastroAtualizacaoParaContasPagas(dados, contaPagaAtual)
                        .ConverterParaContasPagas();

                    ContasPagarDAO.Instance.Update(sessao, contaPagaAtual);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito("Conta paga atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao alterar dados da conta paga.", ex);
                }
            }
        }
    }
}