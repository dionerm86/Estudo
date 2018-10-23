// <copyright file="PostCfopsController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Cfops;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Cfops.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1
{
    /// <summary>
    /// Controller de CFOP.
    /// </summary>
    public partial class CfopsController : BaseController
    {
        /// <summary>
        /// Cadastra um CFOP.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um CFOP.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "CFOP cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarCfop([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var cfop = new ConverterCadastroAtualizacaoParaCfop(dadosParaCadastro)
                        .ConverterParaCfop();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                        .SalvarCfop(cfop);

                    if (!resultado)
                    {
                        sessao.Rollback();
                        return this.ErroValidacao($"Falha ao cadastrar grupo de produto. {resultado.Message.Format()}");
                    }

                    sessao.Commit();
                    return this.Criado("CFOP cadastrado com sucesso!", 0);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar CFOP.", ex);
                }
            }
        }
    }
}
