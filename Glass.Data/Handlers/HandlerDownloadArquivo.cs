using System;
using System.Web;
using Glass.Data.Helper;

namespace Glass.Data.Handlers
{
    public abstract class HandlerDownloadArquivo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // Inclui um cookie para indicar o fim do download
            if (!String.IsNullOrEmpty(context.Request["token"]))
                context.Response.AppendCookie(new HttpCookie("token", context.Request["token"]));

            byte[] arquivo;

            try
            {
                arquivo = ObtemBytesArquivoDownload(context.Request);
                context.Response.ContentType = "application/octet-stream";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + NomeArquivoDownload(context.Request) + "\"");
            }
            catch (Exception ex)
            {
                // Devolve o erro
                context.Response.ContentType = "text/html";
                context.Response.Write(GetErrorResponse(ex));
                return;
            }

            // Escreve o arquivo no buffer
            System.IO.Stream dadosArquivo = new System.IO.MemoryStream(arquivo);

            // Passa o arquivo para download
            byte[] download = new byte[4096];
            int read = 0;
            while ((read = dadosArquivo.Read(download, 0, download.Length)) > 0)
            {
                context.Response.OutputStream.Write(download, 0, read);
                context.Response.Flush();
            }
        }

        private string GetErrorResponse(Exception ex)
        {
            bool debug = UserInfo.GetUserInfo.IsAdminSync;

            string html = debug ? ex.ToString().Replace("\n", "<br>").Replace("\r", "").Replace(" ", "&nbsp;") : @"
                <script type='text/javascript'>
                    alert('" + Glass.MensagemAlerta.FormatErrorMsg("", ex) + @"');
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

        public abstract string NomeArquivoDownload(HttpRequest request);

        public abstract byte[] ObtemBytesArquivoDownload(HttpRequest request);
    }
}