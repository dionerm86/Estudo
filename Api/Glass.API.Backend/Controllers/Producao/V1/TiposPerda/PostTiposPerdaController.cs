// <copyright file="PostTiposPerdaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Producao.TiposPerda;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Producao.V1.TiposPerda.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.TiposPerda
{
    /// <summary>
    /// Controller de tipos de perda.
    /// </summary>
    public partial class TiposPerdaController : BaseController
    {
        /// <summary>
        /// Cadastra um tipo de perda.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um tipo de perda.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Tipo de perda cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarTipoPerda([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var tipoPerda = new ConverterCadastroAtualizacaoParaTipoPerda(dadosParaCadastro)
                        .ConverterParaTipoPerda();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.IPerdaFluxo>()
                        .SalvarTipoPerda(tipoPerda);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar tipo de perda. {resultado.Message.Format()}");
                    }

                    return this.Criado("Tipo de perda cadastrado com sucesso!", tipoPerda.IdTipoPerda);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar tipo de perda.", ex);
                }
            }
        }
    }
}
