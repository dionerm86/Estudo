// <copyright file="PostSetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Producao.Setores;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Producao.V1.Setores.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores.
    /// </summary>
    public partial class SetoresController : BaseController
    {
        /// <summary>
        /// Cadastra um setor.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um setor.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Setor cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarSetor([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var setor = new ConverterCadastroAtualizacaoParaSetor(dadosParaCadastro)
                        .ConverterParaSetor();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<PCP.Negocios.ISetorFluxo>()
                        .SalvarSetor(setor);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar setor. {resultado.Message.Format()}");
                    }

                    return this.Criado("Setor cadastrado com sucesso!", setor.IdSetor);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar setor.", ex);
                }
            }
        }
    }
}
