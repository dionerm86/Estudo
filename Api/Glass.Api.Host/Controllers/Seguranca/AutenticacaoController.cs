using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;
using System.Web.Security;

namespace Glass.Api.Host.Controllers.Seguranca
{
    public class AutenticacaoController : ApiController
    {
        /// <summary>
        /// Autentica um usuário do sistema
        /// Seguranca/Autenticacao/Autenticar?usuario=x&senha=x
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Autenticar(string usuario, string senha)
        {
            try
            {
                var result = ServiceLocator.Current.GetInstance<Api.Seguranca.IAutenticacaoFluxo>()
                    .Autenticar(usuario, senha.HexParaStr());

                FormsAuthentication.SetAuthCookie(result.IdUsuario.ToString(), false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
