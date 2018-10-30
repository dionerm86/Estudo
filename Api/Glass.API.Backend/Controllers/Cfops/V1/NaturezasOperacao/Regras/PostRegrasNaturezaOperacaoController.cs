// <copyright file="PostRegrasNaturezaOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cfops;
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
        /// Cadastra uma regra de natureza de operação.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma regra de natureza de operação.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Regra de natureza de operação cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarRegraNaturezaOperacao([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var regra = new ConverterCadastroAtualizacaoParaRegraNaturezaOperacao(dadosParaCadastro)
                        .ConverterParaRegraNaturezaOperacao();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                        .SalvarRegraNaturezaOperacao(regra);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao cadastrar regra de natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Criado("Regra de natureza de operação cadastrada com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar regra de natureza de operação.", ex);
                }
            }
        }
    }
}
