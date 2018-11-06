// <copyright file="PatchFabricantesFerragemController.cs" company="Sync Softwares">
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
        /// Atualiza um fabricante de ferragem.
        /// </summary>
        /// <param name="id">O identificador do fabricante de ferragem que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no item indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Fabricante de ferragem alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Fabricante de ferragem não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarFabricanteFerragem(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdFabricanteFerragem(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Projeto.Negocios.IFerragemFluxo>();

                    var fabricanteAtual = fluxo.ObterFabricanteFerragem(id);

                    fabricanteAtual = new ConverterCadastroAtualizacaoParaFabricanteFerragem(dadosParaAlteracao, fabricanteAtual)
                        .ConverterParaFabricanteFerragem();

                    var resultado = fluxo.SalvarFabricanteFerragem(fabricanteAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar fabricante de ferragem. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Fabricante de ferragem. atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar fabricante de ferragem..", ex);
                }
            }
        }
    }
}
