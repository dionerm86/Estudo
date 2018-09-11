using System.Web;
using WebGlass.Business.Boleto.Fluxo;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Glass.Financeiro.UI.Web.Process.Handlers
{
    public class Boleto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var envioEmail = context.Request["envioEmail"] == "true";

            var codigoContaBanco = context.Request["codigoContaBanco"].StrParaInt();
            var carteira = context.Request["carteira"];
            var especieDocumento = context.Request["especieDocumento"].StrParaInt();
            var instrucoes = context.Request["instrucoes"].Split(';');

            var codigoNotaFiscal = context.Request["codigoNotaFiscal"].StrParaInt();
            var codigoContaReceber = context.Request["codigoContaReceber"].StrParaInt();
            var codigoLiberacao = context.Request["codigoLiberacao"].StrParaInt();
            var codigoCte = context.Request["codigoCte"].StrParaInt();

            using (var stream = new System.IO.MemoryStream())
            {
                var geradorBoleto = ServiceLocator.Current.GetInstance<IGeradorBoleto>();

                var boletoResultado = geradorBoleto.GerarBoleto(codigoContaReceber, codigoNotaFiscal, codigoLiberacao, codigoCte,
                   codigoContaBanco, carteira, especieDocumento, instrucoes, stream);

                if (boletoResultado)
                {
                    var pdf = stream.ToArray();

                    context.Response.ContentType = "application/pdf";
                    context.Response.OutputStream.Write(pdf, 0, pdf.Length);
                }
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}