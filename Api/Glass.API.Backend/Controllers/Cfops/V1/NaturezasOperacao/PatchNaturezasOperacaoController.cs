// <copyright file="PatchNaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cfops.NaturezasOperacao;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de naturezas de operação.
    /// </summary>
    public partial class NaturezasOperacaoController : BaseController
    {
        /// <summary>
        /// Atualiza uma natureza de operação.
        /// </summary>
        /// <param name="id">O identificador da natureza de operação que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na natureza de operação indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Natureza de operação alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Natureza de operação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarNaturezaOperacao(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdNaturezaOperacao(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>();

                    var naturezaAtual = fluxo.ObtemNaturezaOperacao(id);

                    naturezaAtual = new ConverterCadastroAtualizacaoParaNaturezaOperacao(dadosParaAlteracao, naturezaAtual)
                        .ConverterParaNaturezaOperacao();

                    var resultado = fluxo.SalvarNaturezaOperacao(naturezaAtual);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao atualizar a natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"Natureza de operação atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar a natureza de operação.", ex);
                }
            }
        }
    }
}
