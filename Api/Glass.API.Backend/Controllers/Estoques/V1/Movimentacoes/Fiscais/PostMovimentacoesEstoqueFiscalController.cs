// <copyright file="PostMovimentacoesEstoqueFiscalController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Estoques.Movimentacoes.Fiscais;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Fiscais.CadastroAtualizacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Estoques.V1.Movimentacoes.Fiscais
{
    /// <summary>
    /// Controller de movimentação de estoque fiscal.
    /// </summary>
    public partial class MovimentacoesEstoqueFiscalController : BaseController
    {
        /// <summary>
        /// Cadastra uma movimentação de estoque fiscal (ajuste manual).
        /// </summary>
        /// <param name="dadosParaCadastro">Dados que serão utilizados para cadastrar a movimentação.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Movimentação cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarMovimentacaoEstoqueFiscal([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarCadastroMovimentacaoEstoqueFiscal(sessao, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var movimentacaoEstoqueFiscal = new ConverterCadastroAtualizacaoParaMovimentacaoEstoqueFiscal(dadosParaCadastro)
                        .ConverterParaMovimentacaoEstoqueFiscal();

                    sessao.BeginTransaction();

                    if (dadosParaCadastro.TipoMovimentacao == 1)
                    {
                        MovEstoqueFiscalDAO.Instance.CreditaEstoqueManual(
                            sessao,
                            (uint)movimentacaoEstoqueFiscal.IdProd,
                            (uint)movimentacaoEstoqueFiscal.IdLoja,
                            movimentacaoEstoqueFiscal.QtdeMov,
                            movimentacaoEstoqueFiscal.ValorMov,
                            movimentacaoEstoqueFiscal.DataMov,
                            movimentacaoEstoqueFiscal.Obs);
                    }
                    else if (dadosParaCadastro.TipoMovimentacao == 2)
                    {
                        MovEstoqueFiscalDAO.Instance.BaixaEstoqueManual(
                            sessao,
                            (uint)movimentacaoEstoqueFiscal.IdProd,
                            (uint)movimentacaoEstoqueFiscal.IdLoja,
                            movimentacaoEstoqueFiscal.QtdeMov,
                            movimentacaoEstoqueFiscal.ValorMov,
                            movimentacaoEstoqueFiscal.DataMov,
                            movimentacaoEstoqueFiscal.Obs);
                    }
                    else
                    {
                        return this.ErroValidacao($"O tipo de movimentação deve ser informado.");
                    }

                    sessao.Commit();

                    return this.Criado($"Movimentação de estoque inserida com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar movimentação de estoque.", ex);
                }
            }
        }
    }
}
