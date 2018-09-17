// <copyright file="DeleteProdutosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    public partial class ProdutosController : BaseController
    {
        /// <summary>
        /// Exclui um produto, se possível.
        /// </summary>
        /// <param name="id">O identificador do produto que será excluído.</param>
        /// <returns>Um status HTTP indicando se o produto foi excluído.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Produto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Produto não encontrado para o `id` informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirProduto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdProduto(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }
            }

            try
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IProdutoFluxo>();

                var produto = fluxo.ObtemProduto(id);
                var resultado = fluxo.ApagarProduto(produto);

                if (!resultado)
                {
                    return this.ErroValidacao($"Erro ao excluir o produto. {resultado.Message}");
                }

                return this.Aceito("Produto excluído com sucesso!");
            }
            catch (Exception e)
            {
                return this.ErroValidacao("Erro ao excluir o produto.", e);
            }
        }
    }
}
