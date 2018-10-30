// <copyright file="PostPlanosContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1
{
    /// <summary>
    /// Controller de planos de conta.
    /// </summary>
    public partial class PlanosContaController : BaseController
    {
        /// <summary>
        /// Cadastra um plano de conta.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um plano de conta.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Plano de conta cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarPlanoConta([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var planoConta = new ConverterCadastroAtualizacaoParaPlanoConta(dadosParaCadastro)
                        .ConverterParaPlanoConta();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                        .SalvarPlanoContas(planoConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar plano de conta. {resultado.Message.Format()}");
                    }

                    return this.Criado("Plano de conta cadastrado com sucesso!", planoConta.IdConta);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar plano de conta.", ex);
                }
            }
        }
    }
}
