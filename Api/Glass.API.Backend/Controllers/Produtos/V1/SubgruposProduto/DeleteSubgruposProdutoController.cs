// <copyright file="DeleteSubgruposProdutoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um subgrupo de produto.
        /// </summary>
        /// <param name="id">O identificador do subgrupo de produto que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Subgrupo de produto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Subgrupo de produto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirSubgrupo(int id)
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

                    var subgrupo = fluxo.ObtemSubgrupoProduto(id);

                    var resultado = fluxo.ApagarSubgrupoProduto(subgrupo);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir subgrupo de produto. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Subgrupo de produto excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir subgrupo de produto.", ex);
                }
            }
        }
    }
}
