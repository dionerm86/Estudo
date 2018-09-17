// <copyright file="PostCaixaDiarioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Caixas.Diario.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Caixas.Diario.V1
{
    /// <summary>
    /// Controller de caixa diário.
    /// </summary>
    public partial class CaixaDiarioController : BaseController
    {
        /// <summary>
        /// Reabre o caixa da loja informada.
        /// </summary>
        /// <param name="idLoja">O identificador da loja que terá o caixa reaberto.</param>
        /// <returns>Um status HTTP indicando se o caixa foi reaberto.</returns>
        [HttpPost]
        [Route("{idLoja}/reabrir")]
        [SwaggerResponse(200, "Caixa reaberto.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo idLoja ou de validação na reabertura do caixa.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Loja não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult Reabrir(int idLoja)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdLoja(sessao, idLoja);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    CaixaDiarioDAO.Instance.ReabrirCaixa(sessao, (uint)idLoja);

                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao reabrir caixa.", ex);
                }
            }
        }

        /// <summary>
        /// Fecha o caixa da loja informada.
        /// </summary>
        /// <param name="idLoja">O identificador da loja que terá o caixa fechado.</param>
        /// <param name="fechamentoDto">Dados necessários para fechamento do caixa.</param>
        /// <returns>Um status HTTP indicando se o caixa foi fechado.</returns>
        [HttpPost]
        [Route("{idLoja}/fechar")]
        [SwaggerResponse(202, "Caixa fechado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo idLoja ou de validação no fechamento do caixa.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Loja não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult Fechar(int idLoja, [FromBody]FechamentoDto fechamentoDto)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdLoja(sessao, idLoja);

                if (validacao != null)
                {
                    return validacao;
                }

                if (fechamentoDto.ValorATransferirCaixaGeral == null)
                {
                    return this.ErroValidacao("Informe o valor a ser transferido para o caixa geral");
                }

                try
                {
                    sessao.BeginTransaction();

                    var dataFechamento = fechamentoDto.DiaAnterior ?
                        CaixaDiarioDAO.Instance.GetDataCaixaAberto(sessao, (uint)idLoja) :
                        DateTime.Now;

                    CaixaDiarioDAO.Instance.FechaCaixa(
                        sessao,
                        (uint)idLoja,
                        fechamentoDto.ValorATransferirCaixaGeral.GetValueOrDefault(),
                        dataFechamento,
                        fechamentoDto.DiaAnterior);

                    sessao.Commit();

                    return this.Aceito("Caixa fechado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao fechar caixa.", ex);
                }
            }
        }
    }
}
