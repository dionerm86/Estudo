using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Areas.Graficos.Controllers
{
    public class AdministrativosController : ApiController
    {

        /// <summary>
        /// Grafico DRE
        /// Graficos/Administrativos/DRE
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult Dre()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Administrativos.IAdministrativoFluxo>()
                    .ObtemDre();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Ponto de Equilibrio
        /// Graficos/Administrativos/PontoEquilibrio
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult PontoEquilibrio()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Administrativos.IAdministrativoFluxo>()
                    .ObtemPontoEquilibrio();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Tempo Gasto por Etapa
        /// Graficos/Administrativos/TempoGastoPorEtapa
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult TempoGastoPorEtapa()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Administrativos.IAdministrativoFluxo>()
                    .ObtemTempoGastoPorEtapa();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Metragem a Produzir
        /// Graficos/Administrativos/MetragemProduzir
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public IHttpActionResult MetragemProduzir()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Graficos.Administrativos.IAdministrativoFluxo>()
                    .ObtemMetragemProduzir();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
