// <copyright file="PatchLimiteChequePorCpfCnpjController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cheques.LimitePorCpfCnpj;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1.LimitePorCpfCnpj
{
    /// <summary>
    /// Controller de limite de cheques.
    /// </summary>
    public partial class LimiteChequePorCpfCnpjController : BaseController
    {
        /// <summary>
        /// Atualiza dados do limite de cheque.
        /// </summary>
        /// <param name="id">O identificador do limite de cheque que será alterado.</param>
        /// <param name="dados">Os novos dados que serão alterados no limite de cheque indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Limite de cheque por CPF/CNPJ alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Limite de cheque por CPF/CNPJ não encontrado para o cpfCnpj informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarLimiteCheque(int id, [FromBody] CadastroAtualizacaoDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var limiteCheque = LimiteChequeCpfCnpjDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    limiteCheque = new ConverterCadastroAtualizacaoParaLimiteChequeCpfCnpj(dados, limiteCheque)
                        .ConverterParaLimiteCheque();

                    LimiteChequeCpfCnpjDAO.Instance.InsertOrUpdate(sessao, limiteCheque);
                    sessao.Commit();

                    return this.Aceito("Limite de cheque atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar limite de cheque.", ex);
                }
            }
        }
    }
}