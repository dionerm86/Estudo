// <copyright file="PostOrcamentosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Orcamentos.V1
{
    /// <summary>
    /// Controller de orçamentos.
    /// </summary>
    public partial class OrcamentosController : BaseController
    {
        /// <summary>
        /// Gera um pedido a partir do orçamento.
        /// </summary>
        /// <param name="id">O identificador do orçamento que será usado para gerar o pedido.</param>
        /// <returns>Um status HTTP indicando se o pedido foi gerado com seu identificador.</returns>
        [HttpPost]
        [Route("{id}/gerarPedido")]
        [SwaggerResponse(201, "Pedido gerado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na geração do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Orçamento não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult GerarPedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdOrcamento(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var idPedido = PedidoDAO.Instance.GerarPedido(sessao, (uint)id);

                    sessao.Commit();

                    return this.Criado(string.Format("Pedido {0} gerado com sucesso!", idPedido), idPedido);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Gera pedidos a partir do orçamento.
        /// </summary>
        /// <param name="id">O identificador do orçamento que será usado para gerar o pedido.</param>
        /// <returns>Um status HTTP indicando se o pedido foi gerado com seu identificador.</returns>
        [HttpPost]
        [Route("{id}/gerarPedidosAgrupados")]
        [SwaggerResponse(201, "Pedidos gerados.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na geração dos pedidos.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Orçamentos não encontrados para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult GerarPedidosAgrupados(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdOrcamento(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    PedidoDAO.Instance.GerarPedidosAgrupados(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("Pedidos gerados com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Envia email para o cliente com o orçamento.
        /// </summary>
        /// <param name="id">O identificador do orçamento a ser enviado por email.</param>
        /// <returns>Um status HTTP indicando se o email foi enviado.</returns>
        [HttpPost]
        [Route("{id}/enviarEmail")]
        [SwaggerResponse(202, "Email enviado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação no envio do email.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Orçamento não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult EnviarEmail(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdOrcamento(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    Email.EnviaEmailOrcamento(sessao, (uint)id);

                    LogAlteracaoDAO.Instance.LogEnvioEmailOrcamento((uint)id);

                    sessao.Commit();

                    return this.Aceito("O e-mail foi adicionado na fila para ser enviado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }
    }
}
