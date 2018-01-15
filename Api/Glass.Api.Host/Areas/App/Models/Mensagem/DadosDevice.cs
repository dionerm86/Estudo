using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glass.Api.Host.Areas.App.Models.Mensagem
{
    public class DadosDevice
    {
        public int IdCliente { get; set; }

        public string Uuid { get; set; }

        public string Token { get; set; }
    }
}