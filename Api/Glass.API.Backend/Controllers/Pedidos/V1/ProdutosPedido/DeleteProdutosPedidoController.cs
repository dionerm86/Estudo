// <copyright file="DeleteProdutosPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.ProdutosPedido
{
    /// <summary>
    /// Controller de produtos de pedido.
    /// </summary>
    public partial class ProdutosPedidoController : BaseController
    {
        /// <summary>
        /// Exclui um produto do pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="id">O identificador do produto.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Produto excluído do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido ou produto não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult Excluir(int idPedido, int id)
        {
            using (var sessao = new GDATransaction())
            {
                Data.Model.ProdutosPedido produto;
                var validacao = this.ValidarOperacaoId(sessao, idPedido, id, out produto);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ProdutosPedidoDAO.Instance.Delete(sessao, produto, false, true);
                    sessao.Commit();

                    return this.Aceito(string.Format("Produto {0} excluído com sucesso do pedido {1}!", id, idPedido));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao excluir o produto {0} do pedido {1}.", id, idPedido), e);
                }
            }
        }
    }
}
