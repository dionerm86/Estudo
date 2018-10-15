// <copyright file="AuthorizationAttribute.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Glass.Seguranca;
using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Glass.API.Backend.Atributos
{
    /// <summary>
    /// Atributo de autorização da API.
    /// </summary>
    internal class AuthorizationAttribute : AuthorizeAttribute
    {
        /// <inheritdoc/>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            string token = this.RecuperaTokenAutenticacao(actionContext);

            if (token != null)
            {
                try
                {
                    var login = this.ObterDadosLogin(token);

                    if (login != null)
                    {
                        UserInfo.ConfigurarLoginUsuarioGetterThread(() => login);
                        return true;
                    }
                }
                catch
                {
                    // não faz nada
                }
            }

            return false;
        }

        private string RecuperaTokenAutenticacao(HttpActionContext actionContext)
        {
            var auth = actionContext.Request.Headers.Authorization;

            return auth != null && "Bearer".Equals(auth.Scheme)
                ? auth.Parameter
                : null;
        }

        private LoginUsuario ObterDadosLogin(string token)
        {
            try
            {
                var jsonLogin = new Crypto().Decrypt(token);
                var loginExpiracao = JsonConvert.DeserializeObject<LoginUsuarioExpiracao>(jsonLogin);

                if (loginExpiracao != null && loginExpiracao.DataExpiracao > DateTime.Now)
                {
                    return loginExpiracao.Login;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
