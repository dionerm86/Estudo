// <copyright file="PatchProdutosPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos.ProdutosPedido;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao;
using Glass.API.Backend.Models.Pedidos.ProdutosPedido.Observacao;
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
        /// Atualiza os dados de um produto de pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="id">O identificador do produto.</param>
        /// <param name="dadosParaAtualizacao">Objeto com os dados a serem atualizados no produto de pedido.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Produto atualizado no pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido ou produto não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult Atualizar(int idPedido, int id, [FromBody] CadastroAtualizacaoDto dadosParaAtualizacao)
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
                    produto = new ConverterCadastroAtualizacaoParaProdutoPedido(dadosParaAtualizacao, produto)
                        .ConverterParaProdutoPedido();

                    ProdutosPedidoDAO.Instance.UpdateEAtualizaDataEntrega(sessao, produto);
                    sessao.Commit();

                    return this.Aceito(string.Format("Produto {0} atualizado com sucesso no pedido {1}!", id, idPedido));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao atualizar o produto {0} do pedido {1}.", id, idPedido), e);
                }
            }
        }

        /// <summary>
        /// Atualiza a observação de um produto de pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="id">O identificador do produto.</param>
        /// <param name="dadosEntrada">Objeto com a nova observação do produto de pedido.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/observacao")]
        [SwaggerResponse(202, "Produto atualizado no pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido ou produto não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult SalvarObservacao(int idPedido, int id, [FromBody] DadosEntradaDto dadosEntrada)
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
                    ProdutosPedidoDAO.Instance.AtualizaObs(sessao, (uint)id, dadosEntrada?.Observacao);
                    sessao.Commit();

                    return this.Aceito($"Produto {id} atualizado com sucesso no pedido {idPedido}!");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar a observação do produto {id} do pedido {idPedido}.", e);
                }
            }
        }
    }
}
