// <copyright file="PostNaturezasOperacaoController.cs" company="Sync Softwares">
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
    /// Controller de natureza de operação.
    /// </summary>
    public partial class NaturezasOperacaoController : BaseController
    {
        /// <summary>
        /// Cadastra uma natureza de operação.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma natureza de operação.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Natureza de operação cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarNaturezaOperacao([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var regra = new ConverterCadastroAtualizacaoParaNaturezaOperacao(dadosParaCadastro)
                        .ConverterParaNaturezaOperacao();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                        .SalvarNaturezaOperacao(regra);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao cadastrar natureza de operação. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Criado("Natureza de operação cadastrada com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar natureza de operação.", ex);
                }
            }
        }
    }
}
