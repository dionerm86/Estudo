// <copyright file="PostMovimentacaoEstoqueRealController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Estoques.Movimentacoes.Reais;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais.CadastroAtualizacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Controller de movimentação de estoque real.
    /// </summary>
    public partial class MovimentacaoEstoqueRealController : BaseController
    {
        /// <summary>
        /// Cadastra uma movimentação de estoque real (ajuste manual).
        /// </summary>
        /// <param name="dadosParaCadastro">Dados que serão utilizados para cadastrar a movimentação.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Movimentação cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarMovimentacaoEstoqueReal([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarCadastroMovimentacaoEstoqueReal(sessao, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var movimentacaoEstoqueReal = new ConverterCadastroAtualizacaoParaMovimentacaoEstoqueReal(dadosParaCadastro)
                        .ConverterParaMovimentacaoEstoqueReal();

                    sessao.BeginTransaction();

                    if (dadosParaCadastro.TipoMovimentacao == 1)
                    {
                        MovEstoqueDAO.Instance.CreditaEstoqueManual(
                            sessao,
                            movimentacaoEstoqueReal.IdProd,
                            movimentacaoEstoqueReal.IdLoja,
                            movimentacaoEstoqueReal.QtdeMov,
                            movimentacaoEstoqueReal.ValorMov,
                            movimentacaoEstoqueReal.DataMov,
                            movimentacaoEstoqueReal.Obs);
                    }
                    else
                    {
                        MovEstoqueDAO.Instance.BaixaEstoqueManual(
                            sessao,
                            movimentacaoEstoqueReal.IdProd,
                            movimentacaoEstoqueReal.IdLoja,
                            movimentacaoEstoqueReal.QtdeMov,
                            movimentacaoEstoqueReal.ValorMov,
                            movimentacaoEstoqueReal.DataMov,
                            movimentacaoEstoqueReal.Obs);
                    }

                    sessao.Commit();

                    return this.Criado($"Movimentação de estoque real inserida com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar movimentação de estoque real.", ex);
                }
            }
        }
    }
}
