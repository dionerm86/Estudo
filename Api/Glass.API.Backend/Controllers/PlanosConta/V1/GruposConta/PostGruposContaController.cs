// <copyright file="PostGruposContaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.PlanosConta.GruposConta;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.PlanosConta.V1.GruposConta.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.PlanosConta.V1.GruposConta
{
    /// <summary>
    /// Controller de grupos de conta.
    /// </summary>
    public partial class GruposContaController : BaseController
    {
        /// <summary>
        /// Cadastra um grupo de conta.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um grupo de conta.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Grupo de conta cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarGrupoConta([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var grupoConta = new ConverterCadastroAtualizacaoParaGrupoConta(dadosParaCadastro)
                        .ConverterParaGrupoConta();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Financeiro.Negocios.IPlanoContasFluxo>()
                        .SalvarGrupoConta(grupoConta);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar grupo de conta. {resultado.Message.Format()}");
                    }

                    return this.Criado("Grupo de conta cadastrado com sucesso!", grupoConta.IdGrupo);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar grupo de conta.", ex);
                }
            }
        }
    }
}
