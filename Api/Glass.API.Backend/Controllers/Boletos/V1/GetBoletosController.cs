// <copyright file="GetBoletosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Boletos.V1.Boleto;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Boletos.V1
{
    /// <summary>
    /// Controller para a exibição de boletos.
    /// </summary>
    public partial class BoletosController : BaseController
    {
        /// <summary>
        /// Retorna uma mensagem indicando se o boleto está impresso.
        /// </summary>
        /// <param name="idNotaFiscal">O identificador da nota fiscal.</param>
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// <param name="idLiberacao">O identificador da liberação.</param>
        /// <param name="idConhecimentoTransporte">O identificador do CT-e.</param>
        /// <returns>Uma mensagem indicando se o boleto está impreso.</returns>
        [HttpGet]
        [Route("mensagemImpresso")]
        [SwaggerResponse(200, "Mensagem indicando se o boleto está impresso.", Type = typeof(BoletoImpressoDto))]
        public IHttpActionResult ObterMensagemBoletoImpresso(int? idNotaFiscal = null, int? idContaReceber = null, int? idLiberacao = null, int? idConhecimentoTransporte = null)
        {
            using (var sessao = new GDATransaction())
            {
                var boletoImpresso = new BoletoImpressoDto
                {
                    Mensagem = WebGlass.Business.Boleto.Fluxo.Impresso.Instance.MensagemBoletoImpresso(
                        idContaReceber,
                        idNotaFiscal,
                        idLiberacao,
                        idConhecimentoTransporte),
                };

                return this.Item(boletoImpresso);
            }
        }

        /// <summary>
        /// Retorna o id da nota fiscal com base no id conta receber.
        /// </summary>
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// <returns>O identificador da nota fiscal.</returns>
        [HttpGet]
        [Route("obterIdNotaFiscalPeloIdContaReceber")]
        [SwaggerResponse(200, "Identificador da nota fiscal encontrado.", Type = typeof(NotaFiscalDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da obtenção do identificador da nota fiscal.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterIdNotaFiscalPeloIdContaReceber(int idContaReceber)
        {
            using (var sessao = new GDATransaction())
            {
                if (idContaReceber <= 0)
                {
                    return this.ErroValidacao("Conta a receber não informada.");
                }

                var idsNotasFiscaisContaReceber = NotaFiscalDAO.Instance.ObtemIdNfByContaR(
                    (uint)idContaReceber,
                    true);

                return this.Item(
                    new NotaFiscalDto()
                    {
                        IdNotaFiscal = (int?)idsNotasFiscaisContaReceber?.FirstOrDefault(),
                    });
            }
        }

        /// <summary>
        /// Valida se um boleto pode ser impresso.
        /// </summary>
        /// <param name="idNotaFiscal">O identificador da nota fiscal.</param>
        /// <returns>Um status HTTP indicando se o boleto pode ser impresso.</returns>
        [HttpGet]
        [Route("validarImpressao")]
        [SwaggerResponse(200, "Boleto pode ser impresso.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da impressão do boleto.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ValidarBoleto(int idNotaFiscal)
        {
            if (!FinanceiroConfig.UsarNumNfBoletoSemSeparacao)
            {
                return this.Ok();
            }

            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, idNotaFiscal);

                if (validacao != null)
                {
                    return validacao;
                }

                var idsPedidosNotaFiscal = PedidosNotaFiscalDAO.Instance
                    .GetByNf(sessao, (uint)idNotaFiscal)
                    .Where(f => f.IdPedido.GetValueOrDefault() > 0)
                    .Select(f => f.IdPedido.Value);

                foreach (var idPedido in idsPedidosNotaFiscal)
                {
                    var idLiberarPedido = LiberarPedidoDAO.Instance
                        .ObterIdLiberarPedidoParaImpressaoBoletoNFe(sessao, (int)idPedido, idNotaFiscal);

                    if (!idLiberarPedido.HasValue)
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedido} não possui uma liberação vinculada.");
                    }

                    var pedidosNotaFiscal = PedidosNotaFiscalDAO.Instance
                        .GetByLiberacaoPedido(sessao, (uint)idLiberarPedido.Value);

                    if (!pedidosNotaFiscal.Any())
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois a liberação: {idLiberarPedido} não esta vinculada a mesma.");
                    }

                    var idsPedidosLiberacao = ProdutosLiberarPedidoDAO.Instance
                        .GetIdsPedidoByLiberacao(sessao, (uint)idLiberarPedido.Value);

                    foreach (var idPedLib in idsPedidosLiberacao)
                    {
                        if (!idsPedidosNotaFiscal.Contains(idPedLib))
                        {
                            var notasPedido = PedidosNotaFiscalDAO.Instance
                                .GetByPedido(sessao, idPedLib)
                                .Where(f => f.IdPedido.GetValueOrDefault() > 0 && f.IdNf != idNotaFiscal)
                                .Select(f => f.IdNf)
                                .ToList();

                            if (notasPedido.Any())
                            {
                                var notas = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(
                                    sessao,
                                    string.Join(",", notasPedido));

                                return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedLib} da Liberação: {idLiberarPedido} esta vinculado a outras Notas Fiscais. NF-e's: {notas}.");
                            }
                        }
                    }
                }

                return this.Ok();
            }
        }
    }
}
