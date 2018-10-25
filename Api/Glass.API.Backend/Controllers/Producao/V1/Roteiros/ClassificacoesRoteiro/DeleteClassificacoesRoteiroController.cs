// <copyright file="DeleteClassificacoesRoteiroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Roteiros.ClassificacoesRoteiro
{
    /// <summary>
    /// Controller de classificações de roteiro.
    /// </summary>
    public partial class ClassificacoesRoteiroController : BaseController
    {
        /// <summary>
        /// Exclui uma classificação de roteiro.
        /// </summary>
        /// <param name="id">O identificador da classificação de roteiro que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Classificação de roteiro excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Classificação de roteiro não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirClassificacaoRoteiro(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdClassificacaoRoteiro(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>();

                    var classificacao = fluxo.ObtemClassificacao(id);

                    var resultado = fluxo.ApagarClassificacao(classificacao);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir classificação de roteiro. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Classificação de roteiro excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir classificação de roteiro.", ex);
                }
            }
        }
    }
}
