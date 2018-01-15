using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.DataProtection;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Owin.Cors;
using System.Threading;

namespace Glass.Api.Host
{
    public partial class Startup
    {
        /// <summary>
        /// Opções de autenticação.
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>
        /// Identificador público do cliente.
        /// </summary>
        public static string PublicClientId { get; private set; }

        /// <summary>
        /// Configura os dados de autenticação.
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configura o gerenciador de usuários para usar uma única instancia no sitema,
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            var corsPolicy = new System.Web.Http.Cors.EnableCorsAttribute(
                    "*",
                    "*",
                    "*");

            app.UseCors(new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = request =>
                    {
                        return request.Path.Value == "/token/" ? corsPolicy.GetCorsPolicyAsync(null, CancellationToken.None) : Task.FromResult<System.Web.Cors.CorsPolicy>(null);
                    }
                }
            });
            
            // Habilita a aplicação usasr um cookie para armazenar informações do usuário assinalado
            // e usar um cooke para armazenar temporariamente informações sobre o login com
            // a terceira parte do provedor de login
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token/"),
                Provider = new Providers.ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = false
            };

            // Habilita a aplicação user o bearer token para autenticar usuários
            app.UseOAuthBearerTokens(OAuthOptions);

            Data.Helper.UserInfo.ConfigurarLoginUsuarioGetter(ObterLoginUsuario);
        }

        /// <summary>
        /// Recupera as informações do login do usuário com base no Claims
        /// </summary>
        /// <returns></returns>
        public static Data.Helper.LoginUsuario ObterLoginUsuario()
        {
            var user = (System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal);

            if (user != null)
            {
                var nome = user.FindFirst(ClaimTypes.Name)?.Value;
                var codUser = uint.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                uint idCliente = 0;

                uint.TryParse(user.FindFirst(Glass.Seguranca.ClaimTypes.IdCliente)?.Value, out idCliente);
                var tipoUsuario = uint.Parse(user.FindFirst(Glass.Seguranca.ClaimTypes.TipoUsuario)?.Value ?? "0");
                var idLoja = uint.Parse(user.FindFirst(Glass.Seguranca.ClaimTypes.IdLoja)?.Value ?? "0");

                return new Data.Helper.LoginUsuario
                {
                    Nome = nome,
                    CodUser = codUser,
                    IdCliente = idCliente,
                    TipoUsuario = tipoUsuario,
                    IdLoja = idLoja
                };
            }

            return null;

        }
    }   
}