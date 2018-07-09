// <copyright file="PostProdutosPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos.ProdutosPedido;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao;
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
        /// Cadastra um produto no pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="dadosParaCadastro">Objeto com os dados a serem cadastrados no produto de pedido.</param>
        /// <returns>Um status HTTP informando o resultado da operação.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Ambiente inserido no pedido.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo idPedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult Cadastrar(int idPedido, [FromBody] CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdPedido(sessao, idPedido);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var produto = new ConverterCadastroAtualizacaoParaProdutoPedido(dadosParaCadastro)
                        .ConverterParaProdutoPedido();

                    produto.IdPedido = (uint)idPedido;

                    var idProduto = ProdutosPedidoDAO.Instance.Insert(sessao, produto);
                    sessao.Commit();

                    return this.Criado(string.Format("Produto cadastrado com sucesso no pedido {0}!", idPedido), idProduto);
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao inserir o produto no pedido {0}.", idPedido), e);
                }
            }
        }
    }
}
