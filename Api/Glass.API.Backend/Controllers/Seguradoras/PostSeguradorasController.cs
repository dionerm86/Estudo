// <copyright file="PostSeguradorasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Helper.Seguradoras;
using Glass.API.Backend.Models.Seguradoras.V1.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Seguradoras.V1
{
    /// <summary>
    /// Controller de seguradoras.
    /// </summary>
    public partial class SeguradorasController : BaseController
    {
        /// <summary>
        /// Cadastra uma seguradora.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de uma seguradora.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Seguradora cadastrada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarSeguradora([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var seguradora = new ConverterCadastroAtualizacaoParaSeguradora(dadosParaCadastro)
                        .ConverterParaSeguradora();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Fiscal.Negocios.ICTeFluxo>()
                        .SalvarSeguradora(seguradora);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar seguradora. {resultado.Message.Format()}");
                    }

                    return this.Criado("Seguradora cadastrada com sucesso!", seguradora.IdSeguradora);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar seguradora.", ex);
                }
            }
        }
    }
}
