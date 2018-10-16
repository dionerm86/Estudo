// <copyright file="PatchAmbientesPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos.AmbientesPedido;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.CadastroAtualizacao;
using Glass.Data.DAL;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1.AmbientesPedido
{
    /// <summary>
    /// Controller de ambientes de pedido.
    /// </summary>
    public partial class AmbientesPedidoController : BaseController
    {
        /// <summary>
        /// Atualiza os dados de um ambiente de pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="id">O identificador do ambiente.</param>
        /// <param name="dadosParaAtualizacao">Objeto com os dados a serem atualizados no ambiente de pedido.</param>
        /// <returns>Um status HTTP com o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Ambiente atualizado no pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido dos campos idPedido ou id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido ou ambiente não encontrados.", Type = typeof(MensagemDto))]
        public IHttpActionResult Atualizar(int idPedido, int id, [FromBody] CadastroAtualizacaoDto dadosParaAtualizacao)
        {
            using (var sessao = new GDATransaction())
            {
                AmbientePedido ambiente;
                var validacao = this.ValidarOperacaoId(sessao, idPedido, id, out ambiente);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ambiente = new ConverterCadastroAtualizacaoParaAmbientePedido(dadosParaAtualizacao, ambiente)
                        .ConverterParaAmbientePedido();

                    AmbientePedidoDAO.Instance.Update(sessao, ambiente);
                    sessao.Commit();

                    return this.Aceito(string.Format("Ambiente {0} atualizado com sucesso no pedido {1}!", id, idPedido));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao atualizar o ambiente {0} do pedido {1}.", id, idPedido), e);
                }
            }
        }
    }
}
