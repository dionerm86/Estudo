// <copyright file="PostFabricantesFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.Ferragens.FabricantesFerragem;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.Ferragens.FabricantesFerragem.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Controller de fabricantes de ferragem.
    /// </summary>
    public partial class FabricantesFerragemController : BaseController
    {
        /// <summary>
        /// Cadastra um fabricante de ferragem.
        /// </summary>
        /// <param name="dadosParaCadastro">Objeto contendo dados para inserção de um fabricante de ferragem.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Fabricante de ferragem cadastrado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarSetor([FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var fabricante = new ConverterCadastroAtualizacaoParaFabricanteFerragem(dadosParaCadastro)
                        .ConverterParaFabricanteFerragem();

                    var resultado = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>()
                        .SalvarFabricanteFerragem(fabricante);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao cadastrar fabricante de ferragem. {resultado.Message.Format()}");
                    }

                    return this.Criado("Fabricante de ferragem cadastrado com sucesso!", fabricante.IdFabricanteFerragem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao cadastrar fabricante de ferragem.", ex);
                }
            }
        }
    }
}
