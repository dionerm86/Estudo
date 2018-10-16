// <copyright file="PostAmbientesPedidoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos.AmbientesPedido;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.CadastroAtualizacao;
using Glass.Data.DAL;
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
        /// Cadastra um ambiente no pedido.
        /// </summary>
        /// <param name="idPedido">O identificador do pedido.</param>
        /// <param name="dadosParaCadastro">Objeto com os dados a serem cadastrados no ambiente de pedido.</param>
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
                    sessao.BeginTransaction();

                    var ambiente = new ConverterCadastroAtualizacaoParaAmbientePedido(dadosParaCadastro)
                        .ConverterParaAmbientePedido();

                    ambiente.IdPedido = (uint)idPedido;

                    var idAmbiente = AmbientePedidoDAO.Instance.Insert(sessao, ambiente);
                    sessao.Commit();

                    return this.Criado(string.Format("Ambiente cadastrado com sucesso no pedido {0}!", idPedido), idAmbiente);
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(string.Format("Erro ao inserir o ambiente no pedido {0}.", idPedido), e);
                }
            }
        }
    }
}
