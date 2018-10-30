// <copyright file="DeleteRegrasNaturezaOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao;
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
        /// Exclui um CFOP.
        /// </summary>
        /// <param name="id">O identificador da regra de natureza de operação que será excluída.</param>
        /// <param name="cancelamento">Dto que encapsula o motivo do cancelamento.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Regra de natureza de operação excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Regra de natureza de operação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirRegraNaturezaOperacao(int id, CancelamentoDto cancelamento)
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

                    var regra = fluxo.ObtemRegraNaturezaOperacao(id);

                    var resultado = fluxo.ApagarRegraNaturezaOperacao(regra, cancelamento.Motivo, true);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao excluir regra de natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"Regra de natureza de operação excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir regra de natureza de operação.", ex);
                }
            }
        }
    }
}
