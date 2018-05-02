using Glass.Api.Host.Filters;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Areas.Graficos.Controllers
{
    public class ProducaoController : ApiController
    {
        /// <summary>
        /// Painel da Produção
        /// Graficos/Producao/PainelProducao
        /// </summary>
        /// <returns></returns>
        [HttpGet, AppGraficoAuthAttribute]
        public IHttpActionResult PainelProducao()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Producao.IProducaoFluxo>()
                    .ObtemPainelProducao();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Produção Diária Prevista / Realizada
        /// Graficos/Producao/ProducaoDiariaPrevistaRealizada
        /// </summary>
        /// <returns></returns>
        [HttpGet, AppGraficoAuthAttribute]
        public IHttpActionResult ProducaoDiariaPrevistaRealizada()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Producao.IProducaoFluxo>()
                    .ObtemProducaoDiariaPrevistaRealizada();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Perda por Setor
        /// Graficos/Producao/PerdaPorSetor
        /// </summary>
        /// <returns></returns>
        [HttpGet, AppGraficoAuthAttribute]
        public IHttpActionResult PerdaPorSetor()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Producao.IProducaoFluxo>()
                    .ObtemPerdaPorSetor();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Produção realizada no dia
        /// Graficos/Producao/ProducaoDoDia
        /// </summary>
        /// <returns></returns>
        [HttpGet, AppGraficoAuthAttribute]
        public IHttpActionResult ProducaoDoDia()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Producao.IProducaoFluxo>()
                    .ObtemProducaoDia();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Produção Diária por Setor
        /// Graficos/Producao/ProducaoDiariaPorSetor
        /// </summary>
        /// <returns></returns>
        [HttpGet, AppGraficoAuthAttribute]
        public IHttpActionResult ProducaoDiariaPorSetor()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Producao.IProducaoFluxo>()
                    .ObtemProducaoDiariaPorSetor();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
