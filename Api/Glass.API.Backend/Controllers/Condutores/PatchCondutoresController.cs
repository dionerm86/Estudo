// <copyright file="PatchCondutoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Condutores;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Condutores.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Condutores.V1
{
    /// <summary>
    /// Controller de condutores.
    /// </summary>
    public partial class CondutoresController : BaseController
    {
        /// <summary>
        /// Atualiza um condutor.
        /// </summary>
        /// <param name="id">O identificador do condutor que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no Condutor indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Condutor alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Condutor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarCondutor(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarAtualizacaoCondutor(sessao, id, dadosParaAlteracao);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var condutor = CondutoresDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (condutor == null)
                    {
                        return this.NaoEncontrado($"Condutor {id} não encontrado.");
                    }

                    condutor = new ConverterCadastroAtualizacaoParaCondutor(dadosParaAlteracao, condutor)
                        .ConverterParaCondutor();

                    CondutoresDAO.Instance.Update(sessao, condutor);
                    sessao.Commit();

                    return this.Aceito($"Condutor {id} atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar condutor.", ex);
                }
            }
        }
    }
}
