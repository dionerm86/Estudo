// <copyright file="PatchPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1
{
    /// <summary>
    /// Controller de pedidos.
    /// </summary>
    public partial class PedidosController : BaseController
    {
        /// <summary>
        /// Atualiza os dados de um pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="dadosParaAtualizacao">Os dados que serão atualizados no pedido.</param>
        /// <returns>Um status HTTP que indica se o pedido foi atualizado.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Pedido atualizado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarPedido(int id, [FromBody] CadastroAtualizacaoDto dadosParaAtualizacao)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarAtualizacaoPedido(sessao, id, dadosParaAtualizacao);

                if (validacao != null)
                {
                    return validacao;
                }

                if (dadosParaAtualizacao == null)
                {
                    return this.ErroValidacao("É preciso informar os dados para atualização do pedido.");
                }

                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, id);

                if (pedido == null)
                {
                    return this.NaoEncontrado(string.Format("Pedido {0} não encontrado.", id));
                }

                try
                {
                    pedido = new ConverterCadastroAtualizacaoParaPedido(dadosParaAtualizacao, pedido)
                        .ConverterParaPedido();

                    PedidoDAO.Instance.Update(pedido);

                    sessao.Commit();

                    return this.Aceito(string.Format("Pedido {0} atualizado com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao atualizar o pedido {0}.", id), e);
                }
            }
        }

        /// <summary>
        /// Atualiza a observação de um produto de pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="dadosEntrada">Objeto com as novas observações do pedido.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/alterarObservacoes")]
        [SwaggerResponse(202, "Observação do pedido atualizada sem erros.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarObservacoes(int id, [FromBody] ObservacoesPedidoDto dadosEntrada)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    PedidoDAO.Instance.AtualizaObs(sessao, (uint)id, dadosEntrada?.Observacao, dadosEntrada?.ObservacaoLiberacao);
                    sessao.Commit();

                    return this.Aceito($"Pedido {id} atualizado com sucesso!");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar a observação do pedido {id}.", e);
                }
            }
        }
    }
}
