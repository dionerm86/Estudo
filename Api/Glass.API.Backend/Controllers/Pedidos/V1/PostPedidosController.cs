// <copyright file="PostPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Pedidos;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Pedidos.EnviarValidacaoFinanceiro;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Exceptions;
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
        /// Finaliza um pedido, se possível.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser finalizado.</param>
        /// <returns>Um status HTTP indicando se o pedido foi finalizado.</returns>
        [HttpPost]
        [Route("{id}/finalizar")]
        [SwaggerResponse(202, "Pedido finalizado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(300, "Necessário definir se o pedido será enviado para validação no financeiro.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na finalização do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult FinalizarPedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    PedidoDAO.Instance.FinalizarPedido(sessao, (uint)id, false);

                    // caso a empresa use liberação e seja LITE confirma o pedido automaticamente
                    if (PedidoConfig.LiberarPedido && Geral.SistemaLite && !Geral.ControlePCP)
                    {
                        string script;
                        WebGlass.Business.Pedido.Fluxo.ConfirmarPedido.Instance.ConfirmarPedidoLiberacao(id.ToString(), false, null, false, out script);
                    }

                    sessao.Commit();

                    return this.Aceito($"Pedido {id} finalizado.");
                }
                catch (ValidacaoPedidoFinanceiroException f)
                {
                    return this.MultiplaEscolha(MensagemAlerta.FormatErrorMsg(string.Empty, f));
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Finaliza um pedido, confirmando e gerando conferência, se possível.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser finalizado.</param>
        /// <param name="finalizarConferencia">Define se após gerar a conferência a mesma será finalizada.</param>
        /// <returns>Um status HTTP indicando se o pedido foi finalizado.</returns>
        [HttpPost]
        [Route("{id}/confirmarGerandoConferencia")]
        [SwaggerResponse(202, "Pedido finalizado e gerado conferência.", Type = typeof(MensagemDto))]
        [SwaggerResponse(300, "Necessário definir se o pedido será enviado para validação no financeiro.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na finalização do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ConfirmarGerandoConferencia(int id, bool finalizarConferencia)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    PedidoDAO.Instance.FinalizarPedido(sessao, (uint)id, false);

                    if (PedidoDAO.Instance.ObtemSituacao(sessao, (uint)id) != Data.Model.Pedido.SituacaoPedido.ConfirmadoLiberacao)
                    {
                        var idPedidotmp = string.Empty;
                        var idPedidoErrotmp = string.Empty;
                        PedidoDAO.Instance.ConfirmarLiberacaoPedido(sessao, id.ToString(), out idPedidotmp, out idPedidoErrotmp, false, false);
                    }

                    if (PedidoDAO.Instance.GetTipoPedido(sessao, (uint)id) != Data.Model.Pedido.TipoPedidoEnum.Revenda)
                    {
                        PedidoEspelhoDAO.Instance.GeraEspelho(sessao, (uint)id);
                    }

                    if (finalizarConferencia && PedidoConfig.TelaCadastro.FinalizarConferenciaAoGerarEspelho && PedidoEspelhoDAO.Instance.ExisteEspelho((uint)id))
                    {
                        PedidoEspelhoDAO.Instance.FinalizarPedido(sessao, (uint)id);
                    }

                    sessao.Commit();

                    return this.Aceito($"Pedido {id} finalizado e gerado conferência.");
                }
                catch (ValidacaoPedidoFinanceiroException f)
                {
                    return this.MultiplaEscolha(MensagemAlerta.FormatErrorMsg(string.Empty, f));
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Envia pedido para validação pelo financeiro.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser enviado para o financeiro.</param>
        /// <param name="dadosEntrada">Dados de entrada do método.</param>
        /// <returns>Um status HTTP indicando se o pedido foi enviado para o financeiro.</returns>
        [HttpPost]
        [Route("{id}/enviarValidacaoFinanceiro")]
        [SwaggerResponse(202, "Pedido enviado para o financeiro.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na finalização do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult EnviarValidacaoFinanceiro(int id, [FromBody] DadosEntradaDto dadosEntrada)
        {
            dadosEntrada = dadosEntrada ?? new DadosEntradaDto();

            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    PedidoDAO.Instance.DisponibilizaFinalizacaoFinanceiro(sessao, (uint)id, dadosEntrada.Mensagem);

                    sessao.Commit();

                    return this.Aceito($"Pedido {id} enviado para validação pelo financeiro.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Cria um pedido de produção com base no pedido de revenda inserido.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser gerado um pedido de produção.</param>
        /// <returns>Um status HTTP indicando se o pedido de produção foi criado.</returns>
        [HttpPost]
        [Route("{id}/criarProducaoRevenda")]
        [SwaggerResponse(201, "Pedido de produção criado.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na criação do pedido de produção.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CriarProducaoRevenda(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var idPedidoProducao = PedidoDAO.Instance.CriarPedidoProducaoPedidoRevenda(sessao, PedidoDAO.Instance.GetElementByPrimaryKey(id));
                    sessao.Commit();

                    return this.Criado($"Pedido de produção {idPedidoProducao} criado.", idPedidoProducao);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Reabre um pedido, se possível.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser reaberto.</param>
        /// <returns>Um status HTTP indicando se o pedido foi reaberto.</returns>
        [HttpPost]
        [Route("{id}/reabrir")]
        [SwaggerResponse(202, "Pedido reaberto.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na reabertura do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ReabrirPedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    PedidoDAO.Instance.Reabrir(sessao, (uint)id);
                    sessao.Commit();

                    return this.Aceito($"Pedido {id} reaberto.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Realiza a emissão de um pedido novo.
        /// </summary>
        /// <param name="dadosParaCadastro">Os dados necessários para o cadastro do pedido.</param>
        /// <returns>Um status HTTP que indica se o pedido foi inserido.</returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(201, "Pedido inserido.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult CadastrarPedido([FromBody] Models.Pedidos.CadastroAtualizacao.CadastroAtualizacaoDto dadosParaCadastro)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarCadastroPedido(sessao, dadosParaCadastro);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    var pedido = new ConverterCadastroAtualizacaoParaPedido(dadosParaCadastro)
                        .ConverterParaPedido();

                    var idPedido = PedidoDAO.Instance.Insert(pedido);
                    sessao.Commit();

                    return this.Criado(string.Format("Pedido {0} inserido com sucesso!", idPedido), idPedido);
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao inserir o pedido.", e);
                }
            }
        }

        /// <summary>
        /// Altera a liberação financeira de um pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="dados">Dados de entrada do método.</param>
        /// <returns>Um status HTTP que indica se a liberação financeira do pedido foi alterada.</returns>
        [HttpPost]
        [Route("{id}/alterarLiberacaoFinanceira")]
        [SwaggerResponse(202, "Alteração na liberação financeira do pedido feita sem erros.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na liberação financeira do pedido.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarLiberacaoFinanceira(int id, [FromBody] Models.Pedidos.AlterarLiberacaoFinanceiro.EntradaDto dados)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                if (dados == null)
                {
                    return this.ErroValidacao("Não foram informados os parâmetros de entrada do endpoint.");
                }

                if (!dados.Liberar.HasValue)
                {
                    return this.ErroValidacao("Informação sobre o tipo de liberação financeira não foi passada ao endpoint.");
                }

                try
                {
                    PedidoDAO.Instance.AlteraLiberarFinanc(sessao, (uint)id, dados.Liberar.Value);
                    sessao.Commit();

                    return this.Aceito($"Liberação financeira do pedido {id} alterada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Coloca um pedido em conferência.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <returns>Um status HTTP que indica se o pedido foi colocado em conferência.</returns>
        [HttpPost]
        [Route("{id}/colocarEmConferencia")]
        [SwaggerResponse(202, "Pedido colocado em conferência sem erros.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ColocarEmConferencia(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdPedido(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                if (ProdutosPedidoDAO.Instance.CountInPedido(sessao, (uint)id) == 0)
                {
                    return this.ErroValidacao("Inclua pelo menos um produto no pedido para finalizá-lo.");
                }

                try
                {
                    // Cria um registro na tabela em conferencia para este pedido
                    PedidoConferenciaDAO.Instance.NovaConferencia(
                        sessao,
                        (uint)id,
                        PedidoDAO.Instance.ObtemIdSinal(sessao, (uint)id) > 0);

                    return this.Aceito($"Conferência do pedido {id} gerada com sucesso.");
                }
                catch (Exception ex)
                {
                    return this.ErroValidacao($"Erro ao gerar conferência do pedido {id}.", ex);
                }
            }
        }
    }
}
