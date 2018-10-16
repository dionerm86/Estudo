// <copyright file="PatchAplicacoesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Aplicacoes;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Aplicacoes.V1.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Aplicacoes.V1
{
    /// <summary>
    /// Controller de aplicações (etiqueta).
    /// </summary>
    public partial class AplicacoesController : BaseController
    {
        /// <summary>
        /// Atualiza uma aplicação de etiqueta.
        /// </summary>
        /// <param name="id">O identificador da aplicação que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na aplicação indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Aplicação alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Aplicação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarAplicacao(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarAtualizacaoAplicacao(sessao, id, dadosParaAlteracao);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var aplicacao = EtiquetaAplicacaoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (aplicacao == null)
                    {
                        return this.NaoEncontrado($"Aplicação de etiqueta {id} não encontrada.");
                    }

                    aplicacao = new ConverterCadastroAtualizacaoParaAplicacao(dadosParaAlteracao, aplicacao)
                        .ConverterParaAplicacao();

                    EtiquetaAplicacaoDAO.Instance.Update(sessao, aplicacao);
                    sessao.Commit();

                    return this.Aceito($"Aplicação de etiqueta {id} atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar aplicação.", ex);
                }
            }
        }
    }
}
