// <copyright file="PostContasBancariasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.ContasBancarias;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasBancarias.V1
{
    /// <summary>
    /// Controller de contas bancárias.
    /// </summary>
    public partial class ContasBancariasController : BaseController
    {
        /// <summary>
        /// Cadastra uma conta bancária.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma conta bancária.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Conta bancária cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarContaBancaria([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var contaBancaria = new ConverterCadastroAtualizacaoParaContaBancaria(dadosParaCadastro)
                        .ConverterParaContaBancaria();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>()
                        .SalvarContaBancaria(contaBancaria);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar conta bancária. {resultado.Message.Format()}");
                    }

                    return this.Criado("Conta bancária cadastrada com sucesso!", contaBancaria.IdContaBancaria);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar conta bancária.", ex);
                }
            }
        }
    }
}
