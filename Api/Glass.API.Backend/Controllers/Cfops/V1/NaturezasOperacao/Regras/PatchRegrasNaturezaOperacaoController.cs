// <copyright file="PatchRegrasNaturezaOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cfops.NaturezasOperacao.RegrarNaturezaOperacao;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cfops.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao
{
    /// <summary>
    /// Controller de regras de natureza de operação.
    /// </summary>
    public partial class RegrasNaturezaOperacaoController : BaseController
    {
        /// <summary>
        /// Atualiza uma regra de natureza de operação.
        /// </summary>
        /// <param name="id">O identificador da regra de natureza de operação que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na regra de natureza de operação indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Regra de natureza de operação alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Regra de natureza de operação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarRegraNaturezaOperacao(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdRegraNaturezaOperacao(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>();

                    var regraAtual = fluxo.ObtemRegraNaturezaOperacao(id);

                    regraAtual = new ConverterCadastroAtualizacaoParaRegraNaturezaOperacao(dadosParaAlteracao, regraAtual)
                        .ConverterParaRegraNaturezaOperacao();

                    var resultado = fluxo.SalvarRegraNaturezaOperacao(regraAtual);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao atualizar a regra de natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"Regra de natureza de operação atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar a regra de natureza de operação.", ex);
                }
            }
        }
    }
}
