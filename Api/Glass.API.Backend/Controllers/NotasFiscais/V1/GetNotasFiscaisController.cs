// <copyright file="GetNotasFiscaisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.NotasFiscais.Boleto;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.NotasFiscais.V1
{
    /// <summary>
    /// Controller de liberações.
    /// </summary>
    public partial class NotasFiscaisController : BaseController
    {
        /// <summary>
        /// Valida se um boleto pode ser impresso.
        /// <param name="id">O identificador da nota fiscal.</param>
        /// </summary>
        /// <returns>Um status HTTP indicando se o boleto pode ser impresso.</returns>
        [HttpGet]
        [Route("{id}/validarBoleto")]
        [SwaggerResponse(200, "Boleto pode ser impresso.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da impressão do boleto.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ValidarBoleto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                if (!FinanceiroConfig.UsarNumNfBoletoSemSeparacao)
                {
                    return this.Ok();
                }

                var idsPedidosNf = PedidosNotaFiscalDAO.Instance
                    .GetByNf(sessao, (uint)id)
                    .Where(f => f.IdPedido.GetValueOrDefault(0) > 0)
                    .Select(f => f.IdPedido.Value);

                foreach (var idPedNf in idsPedidosNf)
                {
                    var idLiberarPedido = LiberarPedidoDAO.Instance.ObterIdLiberarPedidoParaImpressaoBoletoNFe(sessao, (int)idPedNf, id).GetValueOrDefault();

                    if (idLiberarPedido == 0)
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedNf} não possui uma liberação vinculada.");
                    }

                    if (PedidosNotaFiscalDAO.Instance.GetByLiberacaoPedido(sessao, (uint)idLiberarPedido).Length == 0)
                    {
                        return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois a liberação: {idLiberarPedido} não esta vinculada a mesma.");
                    }

                    foreach (var idPedLib in ProdutosLiberarPedidoDAO.Instance.GetIdsPedidoByLiberacao(sessao, (uint)idLiberarPedido))
                    {
                        if (!idsPedidosNf.Contains(idPedLib))
                        {
                            var notasPedido = PedidosNotaFiscalDAO.Instance
                                .GetByPedido(sessao, idPedLib)
                                .Where(f => f.IdPedido.GetValueOrDefault(0) > 0 && f.IdNf != id)
                                .Select(f => f.IdNf).ToList();

                            if (notasPedido?.Count() > 0)
                            {
                                var notas = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(sessao, string.Join(",", notasPedido));

                                return this.ErroValidacao($"Não é possível gerar o boleto desta NF-e, pois o pedido: {idPedLib} da Liberação: {idLiberarPedido} esta vinculado a outras Notas Fiscais. NF-e's: {notas}.");
                            }
                        }
                    }
                }

                return this.Ok();
            }
        }

        /// <summary>
        /// Retorna o id da nota fiscal com base no id conta receber.
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// </summary>
        /// <returns>O identificador da nota fiscal.</returns>
        [HttpGet]
        [Route("obterIdNotaFiscalPeloIdContaReceber")]
        [SwaggerResponse(200, "Identificador da nota fiscal encontrado.", Type = typeof(NotaFiscalDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da obtenção do identificador da nota fiscal.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterIdNotaFiscalPeloIdContaReceber(int idContaReceber)
        {
            using (var sessao = new GDATransaction())
            {
                if (idContaReceber == 0)
                {
                    return this.ErroValidacao("Conta a receber não informada.");
                }

                var idsNotaFiscal = NotaFiscalDAO.Instance.ObtemIdNfByContaR((uint)idContaReceber, true);

                var id = idsNotaFiscal != null && idsNotaFiscal.Count > 0 ? (int)idsNotaFiscal[0] : (int?)null;

                return this.Item(
                    new NotaFiscalDto()
                    {
                        IdNotaFiscal = id,
                    });
            }
        }

        /// <summary>
        /// Retorna uma mensagem indicando se o boleto está impresso.
        /// <param name="id">O identificador da nota fiscal.</param>
        /// <param name="idContaReceber">O identificador da conta a receber.</param>
        /// <param name="idLiberacao">O identificador da liberação.</param>
        /// </summary>
        /// <returns>Uma mensagem indicando se o boleto está impreso.</returns>
        [HttpGet]
        [Route("{id}/obterMensagemBoletoImpresso")]
        [SwaggerResponse(200, "Mensagem indicando se o boleto está impresso.", Type = typeof(BoletoImpressoDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação da obtenção do identificador da nota fiscal.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Nota fiscal não encontrada.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterMensagemBoletoImpresso(int id, int idContaReceber, int idLiberacao)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdNotaFiscal(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                var mensagem = WebGlass.Business.Boleto.Fluxo.Impresso.Instance.MensagemBoletoImpresso(idContaReceber, id, idLiberacao);

                return this.Item(
                    new BoletoImpressoDto()
                    {
                        Mensagem = mensagem,
                    });
            }
        }
    }
}
