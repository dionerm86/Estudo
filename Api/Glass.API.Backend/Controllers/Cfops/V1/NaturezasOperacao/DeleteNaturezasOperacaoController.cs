// <copyright file="DeleteNaturezasOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de natureza de operação.
    /// </summary>
    public partial class NaturezasOperacaoController : BaseController
    {
        /// <summary>
        /// Exclui uma natureza de operação.
        /// </summary>
        /// <param name="id">O identificador da natureza de operação que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Natureza de operação excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Natureza de operação não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirNaturezaOperacao(int id)
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

                    var natureza = fluxo.ObtemNaturezaOperacao(id);

                    var resultado = fluxo.ApagarNaturezaOperacao(natureza);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao excluir natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Aceito($"Natureza de operação excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir natureza de operação.", ex);
                }
            }
        }
    }
}
