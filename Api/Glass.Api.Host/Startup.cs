using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Glass.Api.Host.Startup))]
namespace Glass.Api.Host
{
    public partial class Startup
    {
        /// <summary>
        /// Realiza a configuração da aplicação.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}