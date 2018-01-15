using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;

namespace Glass.Api.Host.Controllers
{
    public class AutenticacaoController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Autenticar(string usuario, string senha)
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Seguranca.IAutenticacaoFluxo>()
                    .Autenticar(usuario, senha);

                return Ok(new
                {
                    Success = true,
                    Usuario = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Usuario = (Seguranca.IUsuario)null,
                    Message = ex.Message
                });
            }
        }
    }
}
