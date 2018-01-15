using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Graficos
{
    public class FinanceirosController : ApiController
    {
        /// <summary>
        /// Previsão Financeira a Receber
        /// Graficos/Financeiros/PrevisaoReceberVencida
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult PrevisaoReceberVencida()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemPrevisaoFinanceiraReceberVencida();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Previsão Financeira a Pagar
        /// Graficos/Financeiros/PrevisaoPagarVencida
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult PrevisaoPagarVencida()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemPrevisaoFinanceiraPagarVencida();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Previsão Financeira a Receber
        /// Graficos/Financeiros/PrevisaoReceber
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult PrevisaoReceber()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemPrevisaoFinanceiraReceber();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Previsão Financeira a Pagar
        /// Graficos/Financeiros/PrevisaoPagar
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult PrevisaoPagar()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemPrevisaoFinanceiraPagar();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Recebimentos por Tipo
        /// Graficos/Financeiros/RecebimentosPorTipo
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult RecebimentosPorTipo()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemRecebimentoPorTipo();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Pagamentos
        /// Graficos/Financeiros/Pagamentos
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult Pagamentos()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Financeiros.IFinanceiroFluxo>()
                    .ObtemPagamentos();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
