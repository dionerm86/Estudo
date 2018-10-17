// <copyright file="PatchSubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.SubgruposProduto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.SubgruposProduto.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.SubgruposProduto
{
    /// <summary>
    /// Controller de subgrupos de produto.
    /// </summary>
    public partial class SubgruposProdutoController : BaseController
    {
        /// <summary>
        /// Atualiza um subgrupo de produto.
        /// </summary>
        /// <param name="id">O identificador do subgrupo de produto que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no subgrupo de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Subgrupo de produto alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Subgrupo de produto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarSubgrupo(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdSubgrupoProduto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>();

                    var subgrupoAtual = fluxo.ObtemSubgrupoProduto(id);

                    subgrupoAtual = new ConverterCadastroAtualizacaoParaSubgrupoProduto(dadosParaAlteracao, subgrupoAtual)
                        .ConverterParaSubgrupoProduto();

                    var resultado = fluxo.SalvarSubgrupoProduto(subgrupoAtual);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar subgrupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Subgrupo de produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar subgrupo de produto.", ex);
                }
            }
        }
    }
}
