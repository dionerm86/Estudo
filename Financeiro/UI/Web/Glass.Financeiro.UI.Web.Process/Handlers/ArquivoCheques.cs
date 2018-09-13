using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Glass.Financeiro.UI.Web.Process.Handlers
{
    /// <summary>
    /// Classe para recuperação de arquivos de exportação do arquivo de cheques.
    /// </summary>
    public class ArquivoCheques : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var arq = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Negocios.IChequesFluxo>()
                    .GerarArquivoCheques(context.Request["idDeposito"].StrParaUint());

                var nomeArquivo = $"DSF { context.Request["idDeposito"] }.txt";

                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nomeArquivo + "\"");

                arq.Salvar(context.Response.OutputStream);
            }
            catch (Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(this.GetErrorResponse(ex));
                context.Response.Write("<script>window.close();</script>");
            }
        }

        private string GetErrorResponse(Exception ex)
        {
            string html = @"
            <script type='text/javascript'>
                alert('" + MensagemAlerta.FormatErrorMsg("", ex) + @"');
                window.history.go(-1);
            </script>";

            return @"
            <html>
                <body>
                    " + html + @"
                </body>
            </html>";
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
