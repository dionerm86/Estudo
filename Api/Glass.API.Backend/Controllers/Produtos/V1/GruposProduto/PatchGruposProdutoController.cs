// <copyright file="PatchGruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.GruposProduto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.GruposProduto.CadastroAtualizacao;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.GruposProduto
{
    /// <summary>
    /// Controller de grupos de produto.
    /// </summary>
    public partial class GruposProdutoController : BaseController
    {
        /// <summary>
        /// Atualiza um grupo de produto.
        /// </summary>
        /// <param name="id">O identificador do grupo de produto que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no grupo de produto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de produto alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de produto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarGrupo(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdGrupoProduto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IGrupoProdutoFluxo>();

                    var grupoAtual = fluxo.ObtemGrupoProduto(id);

                    var grupo = new ConverterCadastroAtualizacaoParaGrupoProduto(dadosParaAlteracao, grupoAtual)
                        .ConverterParaGrupoProduto();

                    grupo.IdGrupoProd = id;

                    var resultado = fluxo.SalvarGrupoProduto(grupo);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao atualizar grupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Grupo de produto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar grupo de produto.", ex);
                }
            }
        }
    }
}
