// <copyright file="PatchContasPagarController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.ContasPagar;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasPagar.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasPagar.V1
{
    /// <summary>
    /// Controller de contas a pagar.
    /// </summary>
    public partial class ContasPagarController : BaseController
    {
        /// <summary>
        /// Altera uma conta a pagar.
        /// </summary>
        /// <param name="id"> O identificador da conta a pagar que será alterada.</param>
        /// <param name="dados"> Os dados utilizados </param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Conta a pagar alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Conta a pagar não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarContaAPagar(int id, [FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdContasPagar(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var contaAPagarAtual = Data.DAL.ContasPagarDAO.Instance.GetElementByPrimaryKey(id);

                    contaAPagarAtual = new ConverterCadastroAtualizacaoParaContasPagar(dados, contaAPagarAtual)
                        .ConverterParaContasPagar();

                    ContasPagarDAO.Instance.Update(sessao, contaAPagarAtual);

                    sessao.Commit();
                    sessao.Close();

                    return this.Aceito("Conta a pagar atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    sessao.Close();
                    return this.ErroValidacao("Erro ao alterar dados da conta a pagar.", ex);
                }
            }
        }
    }
}