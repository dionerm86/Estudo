// <copyright file="PatchChequesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cheques;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cheques.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cheques.V1
{
    /// <summary>
    /// Controller de cheques.
    /// </summary>
    public partial class ChequesController : BaseController
    {
        /// <summary>
        /// Atualiza dados do cheque.
        /// </summary>
        /// <param name="id">O identificador do cheque que será alterado.</param>
        /// <param name="dadosEntrada">Os novos dados que serão alterados no cheque indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/alterarDados")]
        [SwaggerResponse(202, "Dados do cheque atualizados.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cheque não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarDados(int id, [FromBody] CadastroAtualizacaoDto dadosEntrada)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCheque(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var cheque = ChequesDAO.Instance.GetElementByPrimaryKey(sessao, (uint)id);

                    if (cheque == null)
                    {
                        return this.NaoEncontrado($"Cheque não encontrado.");
                    }

                    sessao.BeginTransaction();

                    cheque = new ConverterCadastroAtualizacaoParaCheque(dadosEntrada, cheque)
                        .ConverterParaCheque();

                    ChequesDAO.Instance.AlterarDados(sessao, cheque);
                    sessao.Commit();

                    return this.Aceito("Cheque atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao atualizar cheque.", ex);
                }
            }
        }
    }
}
