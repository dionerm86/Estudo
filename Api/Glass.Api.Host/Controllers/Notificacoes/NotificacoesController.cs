using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Controllers.Notificacoes
{
    public class NotificacoesController : ApiController
    {
        /// <summary>
        /// Obtem as notificações disponiveis
        /// Notificacoes/ObtemNotificacoes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ObtemNotificacoes()
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Notificacoes.INotificacaoFluxo>()
                    .ObtemNotificacoes();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtem as notificações disponiveis
        /// Notificacoes/MarcarEnviadas
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult MarcarEnviadas([FromBody] string id)
        {
            try
            {
                var data = DateTime.Parse(id);
                ServiceLocator.Current.GetInstance<Api.Notificacoes.INotificacaoFluxo>()
                    .MarcarEnviada(data);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
