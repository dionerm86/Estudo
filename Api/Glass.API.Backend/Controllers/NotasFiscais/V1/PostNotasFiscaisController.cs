﻿// <copyright file="PostNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        [SwaggerResponse(200, "Nota fiscal autorizada para uso.")]
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

                    if (mensagem == "NFe está autorizada para uso.")
                    {
                        return this.Ok();
                    }

                    return this.ErroValidacao(mensagem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Nota fiscal autorizada para uso.")]
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

                    if (mensagem == "NFe está autorizada para uso.")
                    {
                        return this.Ok();
                    }

                    return this.ErroValidacao(mensagem);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Nota fiscal reaberta.")]
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

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Nota fiscal complementar gerada.", Type = typeof(CriadoDto<int>))]
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

                    return this.Criado<int>("Nota fiscal complementar gerada.", (int)idNf);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Nota fiscal FS-DA emitida.")]
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

                    NotaFiscalDAO.Instance.EmitirNfFS((uint)id);

                    sessao.Commit();

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Email adicionado na fila de envio.")]
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

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Separação de valores concluída.")]
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

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Cancelamento da separação de valores concluída.")]
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

                    return this.Ok();
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
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
        [SwaggerResponse(200, "Nota fiscal emitida.", Type = typeof(MensagemDto))]
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

                    var retornoEmissao = NotaFiscalDAO.Instance.EmitirNfcOffline((uint)id);

                    sessao.Commit();

                    return this.Aceito(retornoEmissao);
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Emite um NFC-e em modo offlines.
        /// </summary>
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se a emissão da NFC-e foi realizada.</returns>
        [HttpPost]
        [Route("{id}/emitirNfceOffline")]
        [SwaggerResponse(200, "Nota fiscal emitida em modo offline.", Type = typeof(MensagemDto))]
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

                    var retornoEmissao = NotaFiscalDAO.Instance.EmitirNf((uint)id, false, true, false);

                    sessao.Commit();

                    return this.Aceito(retornoEmissao);
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
