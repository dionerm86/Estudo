// <copyright file="PostNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.NotasFiscais.V1.Contingencia;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.NotasFiscais.V1
{
    /// <summary>
    /// Controller de notas fiscais.
    /// </summary>
    public partial class NotasFiscaisController : BaseController
    {
        /// <summary>
        /// Consulta a situação do lote da nota fiscal.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando o status do lote.</returns>
        [HttpPost]
        [Route("{id}/consultarSituacaoLote")]
        [SwaggerResponse(202, "Nota fiscal processada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na consulta do lote.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ConsultarSituacaoLote(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var mensagem = Data.NFeUtils.ConsultaSituacao.ConsultaLote((uint)id);

                    sessao.Commit();

                    return this.Aceito(mensagem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao consultar situação do lote.", ex);
                }
            }
        }

        /// <summary>
        /// Consulta a situação da nota fiscal.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando o status do lote.</returns>
        [HttpPost]
        [Route("{id}/consultarSituacao")]
        [SwaggerResponse(202, "Nota fiscal processada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na consulta da situação da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ConsultarSituacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var mensagem = Data.NFeUtils.ConsultaSituacao.ConsultaSitNFe((uint)id);

                    sessao.Commit();

                    return this.Aceito(mensagem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao consultar situação da nota fiscal", ex);
                }
            }
        }

        /// <summary>
        /// Reabre uma nota fiscal de terceiros.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando o status do lote.</returns>
        [HttpPost]
        [Route("{id}/reabrir")]
        [SwaggerResponse(202, "Nota fiscal reaberta.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na reabertura da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult Reabrir(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    NotaFiscalDAO.Instance.ReabrirNotaEntradaTerceiros((uint)id);
                    sessao.Commit();

                    return this.Aceito($"Nota fiscal {id} reaberta.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao reabrir nota fiscal.", ex);
                }
            }
        }

        /// <summary>
        /// Gera uma nota fiscal complementar da nota informada.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando o status do lote.</returns>
        [HttpPost]
        [Route("{id}/gerarNotaFiscalComplementar")]
        [SwaggerResponse(201, "Nota fiscal complementar gerada.", Type = typeof(CriadoDto<int>))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na geração da nota fiscal complementar.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult GerarNotaFiscalComplementar(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var idNf = NotaFiscalDAO.Instance.GeraNFeComplementar((uint)id);
                    sessao.Commit();

                    return this.Criado("Nota fiscal complementar gerada.", (int)idNf);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao gerar nota complementar.", ex);
                }
            }
        }

        /// <summary>
        /// Emitir nota fiscal FS-DA.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando o status do lote.</returns>
        [HttpPost]
        [Route("{id}/emitirNotaFiscalFsda")]
        [SwaggerResponse(202, "Nota fiscal FS-DA processada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na emissão da nota fiscal FS-DA.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult EmitirNotaFiscalFsda(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var retorno = NotaFiscalDAO.Instance.EmitirNfFS((uint)id);
                    sessao.Commit();

                    return this.Aceito(retorno);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao emitir nota fiscal.", ex);
                }
            }
        }

        /// <summary>
        /// Reenvia o email da nota fiscal.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal que será reenviado o email.</param>
        /// <param name="cancelamento">Define se é para enviar email de cancelamento da nota.</param>
        /// <returns>Um status HTTP indicando se o email foi inserido na fila de envio.</returns>
        [HttpPost]
        [Route("{id}/reenviarEmail")]
        [SwaggerResponse(202, "Email adicionado à fila de envio.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação no reenvio de email de nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ReenviarEmail(int id, bool cancelamento)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    NotaFiscalDAO.Instance.EnviarEmailXml(sessao, NotaFiscalDAO.Instance.GetElement(sessao, (uint)id), cancelamento);
                    sessao.Commit();

                    return this.Aceito("E-mail adicionado à fila de envio.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao reenviar e-mail", ex);
                }
            }
        }

        /// <summary>
        /// Separa valores de contas a receber da liberação e da nota fiscal.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se a separação de valores foi realizada.</returns>
        [HttpPost]
        [Route("{id}/separarValores")]
        [SwaggerResponse(202, "Separação de valores concluída.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na separação de valores de contas a receber da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult SepararValores(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(sessao, (uint)id);

                    if (tipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
                    {
                        if (NotaFiscalDAO.Instance.ObtemIdCliente(sessao, (uint)id).GetValueOrDefault() == 0)
                        {
                            return this.ErroValidacao("Não é possível fazer a vinculação de valores sem cliente.");
                        }

                        if (!new SeparacaoValoresFiscaisEReaisContasReceber().Separar(sessao, (uint)id))
                        {
                            return this.ErroValidacao("Não foram encontradas contas a receber para realizar a vinculação.");
                        }
                    }

                    if (tipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros && !new SeparacaoValoresFiscaisEReaisContasPagar().Separar(sessao, (uint)id))
                    {
                        return this.ErroValidacao("Não foram encontradas contas a pagar para realizar a vinculação.");
                    }

                    sessao.Commit();

                    return this.Aceito($"Separação de valores da nota fiscal {id} concluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao vincular valores.", ex);
                }
            }
        }

        /// <summary>
        /// Cancela a separação de valores de contas a receber da liberação e da nota fiscal.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se o cancelamento da separação de valores foi realizada.</returns>
        [HttpPost]
        [Route("{id}/cancelarSeparacaoValores")]
        [SwaggerResponse(202, "Cancelamento da separação de valores concluída.")]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação no cancelamento da separação de valores de contas a receber da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarSeparacaoValores(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var tipoDocumento = NotaFiscalDAO.Instance.GetTipoDocumento(sessao, (uint)id);

                    if (tipoDocumento == (int)NotaFiscal.TipoDoc.Saída)
                    {
                        new SeparacaoValoresFiscaisEReaisContasReceber().Cancelar(sessao, (uint)id);
                        NotaFiscalDAO.Instance.DesvinculaReferenciaPedidosAntecipados(sessao, id);
                    }

                    if (tipoDocumento == (int)NotaFiscal.TipoDoc.EntradaTerceiros)
                    {
                        new SeparacaoValoresFiscaisEReaisContasPagar().Cancelar(sessao, (uint)id);
                    }

                    sessao.Commit();

                    return this.Aceito($"Separação de valores da nota fiscal {id} cancelada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao desvincular valores", ex);
                }
            }
        }

        /// <summary>
        /// Emite um NFC-e.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se a emissão da NFC-e foi realizada.</returns>
        [HttpPost]
        [Route("{id}/emitirNfce")]
        [SwaggerResponse(202, "Nota fiscal emitida.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na emissão da nota fiscal de consumidor final.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult EmitirNfce(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var retornoEmissao = NotaFiscalDAO.Instance.EmitirNf((uint)id, false, true, false);
                    sessao.Commit();

                    return this.Aceito(retornoEmissao);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao emitir NFC-e.", ex);
                }
            }
        }

        /// <summary>
        /// Emite um NFC-e em modo offline.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se a emissão da NFC-e foi realizada.</returns>
        [HttpPost]
        [Route("{id}/emitirNfceOffline")]
        [SwaggerResponse(202, "Nota fiscal emitida em modo offline.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo `id` ou de validação na emissão da nota fiscal de consumidor final em modo offline.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult EmitirNfceOffline(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    var retornoEmissao = NotaFiscalDAO.Instance.EmitirNfcOffline((uint)id);
                    sessao.Commit();

                    return this.Aceito(retornoEmissao);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao emitir NFC-e.", ex);
                }
            }
        }

        /// <summary>
        /// Habilita o modo de contingência da nota fiscal.
        /// </summary>
        /// <param name="entrada">Define se o tipo de contingência a ser alterado.</param>
        /// <returns>Um status HTTP indicando se o modo de contingência foi alterado.</returns>
        [HttpPost]
        [Route("alterarContingencia")]
        [SwaggerResponse(202, "Modo de contingência alterado.")]
        [SwaggerResponse(400, "Erro ao habilitar modo de contingência.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarContingencia([FromBody] EntradaDto entrada)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    ConfigDAO.Instance.SetValue(Config.ConfigEnum.ContingenciaNFe, UserInfo.GetUserInfo.IdLoja, (int)entrada.TipoContingencia);
                    sessao.Commit();

                    return this.Aceito("Modo de contingência alterado.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Falha ao habilitar contingência.", ex);
                }
            }
        }
    }
}
