using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Http;
using System.Web.Security;

namespace Glass.Api.Host.Areas.Seguranca.Controllers
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
        public IHttpActionResult Autenticar(string usuario, string senha, bool cliente = false)
        {
            try
            {
                var loginUsuarioGetter = UserInfo.ObterLoginUsuarioGetter();
                UserInfo.ConfigurarLoginUsuarioGetter(null);

                var result = ServiceLocator.Current.GetInstance<Api.Seguranca.IAutenticacaoFluxo>()
                    .Autenticar(usuario, senha.HexParaStr(), cliente);

                FormsAuthentication.SetAuthCookie(GetUserName(result, cliente), false);

                UserInfo.ConfigurarLoginUsuarioGetter(loginUsuarioGetter);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #region Private Methods

        /// <summary>
        /// Recupera o nome do usuário para setar na autenticação.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        private string GetUserName(Api.Seguranca.IUsuario usuario, bool cliente)
        {
            return string.Format("{0}{1}", usuario.IdUsuario.ToString(),
                    cliente ? "|cliente" : string.Empty);
        }

        #endregion
    }
}
