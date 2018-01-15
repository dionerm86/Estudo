using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Graficos
{
    public class VendasController : ApiController
    {
        /// <summary>
        /// Grafico de Vendas (Curva ABC)
        /// Graficos/Vendas/VendasCurvaAbc
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult VendasCurvaAbc()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Vendas.IVendaFluxo>()
                    .ObtemVendasCurvaAbc();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gráfico de Venda por Cliente (Curva ABC)
        /// Graficos/Vendas/VendasPorClienteCurvaAbc
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult VendasPorClienteCurvaAbc()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Vendas.IVendaFluxo>()
                    .ObtemVendasPorClienteCurvaAbc();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gráfico de vendas por pedido
        /// Graficos/Vendas/VendasPorPedido
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult VendasPorPedido()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Vendas.IVendaFluxo>()
                    .ObtemVendasPorPedido();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gráfico de vendas por vendedor
        /// Graficos/Vendas/VendasPorVendedor
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult VendasPorVendedor()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Vendas.IVendaFluxo>()
                    .ObtemVendasPorVendedor();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gráfico de vendas por produto
        /// Graficos/Vendas/VendasPorProduto
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult VendasPorProduto()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Vendas.IVendaFluxo>()
                    .ObtemVendasPorProduto();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
