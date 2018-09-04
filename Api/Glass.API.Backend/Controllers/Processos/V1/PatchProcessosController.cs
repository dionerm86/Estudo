// <copyright file="PatchProcessosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Processos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Processos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Processos.V1
{
    /// <summary>
    /// Controller de processos (etiqueta).
    /// </summary>
    public partial class ProcessosController : BaseController
    {
        /// <summary>
        /// Atualiza um processo de etiqueta.
        /// </summary>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Processo alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Processo não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarProcesso(int id, [FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarAtualizacaoProcesso(sessao, id, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var processo = EtiquetaProcessoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (processo == null)
                    {
                        return this.NaoEncontrado($"Processo de etiqueta {id} não encontrado.");
                    }

                    processo = new ConverterCadastroAtualizacaoParaProcesso(dadosParaCadastro, processo)
                        .ConverterParaProcesso();

                    EtiquetaProcessoDAO.Instance.Update(sessao, processo);
                    sessao.Commit();

                    return this.Aceito($"Processo de etiqueta {id} atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar processo.", ex);
                }
            }
        }
    }
}
