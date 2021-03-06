﻿using Glass.Data.Helper;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Security;

namespace Glass.Api.Host.Filters
{
    public class AppGraficoAuthAttribute : ActionFilterAttribute
    {
        private Func<LoginUsuario> _loginUsuarioGetter;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var cookies = actionContext.Request.Headers.GetCookies().FirstOrDefault();
                var aspAuth = cookies?[FormsAuthentication.FormsCookieName];

                if (aspAuth == null)
                    actionContext.Response = CriarResposta(HttpStatusCode.Unauthorized, Newtonsoft.Json.JsonConvert.SerializeObject(new { Message = "Não autorizado." }));
                else
                {
                    var auth = FormsAuthentication.Decrypt(aspAuth.Value);
                    HttpContext.Current.User = new Colosoft.Security.Principal.DefaultPrincipal(new Colosoft.Security.Principal.DefaultIdentity(auth.Name, null, !auth.Expired));
                    _loginUsuarioGetter = UserInfo.ObterLoginUsuarioGetter();
                    UserInfo.ConfigurarLoginUsuarioGetter(null);
                }
            }
            catch (ArgumentException)
            {
                actionContext.Response = CriarResposta(HttpStatusCode.Unauthorized, Newtonsoft.Json.JsonConvert.SerializeObject(new { Message = "Não autorizado." }));
            }
            catch (Exception ex)
            {
                actionContext.Response = CriarResposta(HttpStatusCode.InternalServerError, Newtonsoft.Json.JsonConvert.SerializeObject(new { Message = ex.Message }));
            }

        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            UserInfo.ConfigurarLoginUsuarioGetter(_loginUsuarioGetter);
        }

        private HttpResponseMessage CriarResposta(HttpStatusCode status, string content)
        {
            return new HttpResponseMessage()
            {
                StatusCode = status,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
    }
}