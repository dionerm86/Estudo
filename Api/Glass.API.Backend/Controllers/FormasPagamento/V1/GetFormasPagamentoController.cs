// <copyright file="GetFormasPagamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Parcelas.V1.Filtro;
using Glass.API.Backend.Models.PlanosConta.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.FormasPagamento.V1
{
    /// <summary>
    /// Controller de formas de pagamento.
    /// </summary>
    public partial class FormasPagamentoController : BaseController
    {
        /// <summary>
        /// Recupera as formas de pagamento para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as formas de pagamento encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Formas de pagamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagamento = FormaPagtoDAO.Instance.GetForControle()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdFormaPagto,
                        Nome = p.Descricao,
                    });

                return this.Lista(formasPagamento);
            }
        }

        /// <summary>
        /// Recupera as formas de pagamento para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as formas de pagamento encontradas.</returns>
        [HttpGet]
        [Route("filtroNotaFiscal")]
        [SwaggerResponse(200, "Formas de pagamento de nota fiscal encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento de nota fiscal não encontradas.")]
        public IHttpActionResult ObterFiltroNotaFiscal()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagamento = FormaPagtoDAO.Instance.GetForNotaFiscal(0)
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdFormaPagto,
                        Nome = p.Descricao,
                    });

                return this.Lista(formasPagamento);
            }
        }

        /// <summary>
        /// Recupera as formas de pagamento para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as formas de pagamento encontradas.</returns>
        [HttpGet]
        [Route("filtroContasRecebidas")]
        [SwaggerResponse(200, "Formas de pagamento de contas recebidas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento de contas recebidas não encontradas.")]
        public IHttpActionResult ObterFiltroContasRecebidas()
        {
            using (var sessao = new GDATransaction())
            {
                var formasPagamento = FormaPagtoDAO.Instance.GetForConsultaConta()
                    .Select(p => new IdNomeDto
                    {
                        Id = (int)p.IdFormaPagto,
                        Nome = p.Descricao,
                    });

                return this.Lista(formasPagamento);
            }
        }

        /// <summary>
        /// Recupera a lista de formas de pagamento de compra.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das formas de pagamentos de compra.</returns>
        [HttpGet]
        [Route("filtroCompras")]
        [SwaggerResponse(200, "Formas de pagamento de compras encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento de compras não encontradas")]
        public IHttpActionResult ObterListaFormasPagamentoCompra()
        {
            try
            {
                var formasPagamentoCompra = FormaPagtoDAO.Instance.GetForCompra()
                    .Select(fpc => new IdNomeDto
                    {
                        Id = (int)fpc.IdFormaPagto,
                        Nome = fpc.Descricao,
                    });

                return this.Lista(formasPagamentoCompra);
            }
            catch (Exception ex)
            {
                return this.ErroValidacao("Erro ao obter lista de formas de pagamento de compras.", ex);
            }
        }

        /// <summary>
        /// Recupera a lista de formas de pagamento de pagamentos.
        /// </summary>
        /// <returns>Uma lista JSON com os dados das formas de pagamentos de pagamentos.</returns>
        [HttpGet]
        [Route("filtroPagamentos")]
        [SwaggerResponse(200, "Formas de pagamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento não encontradas.")]
        public IHttpActionResult ObterListaFormasPagamentoPagamentos()
        {
            try
            {
                var formasPagamentoPagamentos = FormaPagtoDAO.Instance.GetForPagto(false)
                    .Select(fpp => new IdNomeDto
                    {
                        Id = (int)fpp.IdFormaPagto,
                        Nome = fpp.Descricao,
                    });

                return this.Lista(formasPagamentoPagamentos);
            }
            catch (Exception ex)
            {
                return this.ErroValidacao("Erro ao obter lista de formas de pagamento de pagamentos.", ex);
            }
        }
    }
}
