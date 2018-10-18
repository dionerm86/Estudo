// <copyright file="PostAplicacoesController.cs" company="Sync Softwares">
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
        /// Cadastra uma aplicação de etiqueta.
        /// </summary>
        /// <param name="dadosParaCadastro">Os dados para o cadastro da aplicação.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Aplicação cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarAplicacao([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarCadastroAplicacao(sessao, dadosParaCadastro);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var aplicacao = new ConverterCadastroAtualizacaoParaAplicacao(dadosParaCadastro)
                        .ConverterParaAplicacao();

                    sessao.BeginTransaction();

                    var idAplicacao = EtiquetaAplicacaoDAO.Instance.Insert(sessao, aplicacao);
                    sessao.Commit();

                    return this.Criado($"Aplicação de etiqueta {idAplicacao} inserida com sucesso!", idAplicacao);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao inserir aplicação.", ex);
                }
            }
        }
    }
}
