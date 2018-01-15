<%@ WebHandler Language="C#" Class="ArquivoFCI" %>

using System;
using System.Web;
using System.IO;

public class ArquivoFCI : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        try
        {
            var idArquivoFci = context.Request["idArquivoFci"];
            var retorno = bool.Parse(context.Request["retorno"]);

            // Verifica se o arquivo da NF-e existe
            string path = Glass.Data.Helper.Utils.GetArquivoFCIPath + idArquivoFci + (retorno ? "_retorno" : "") + ".txt";
            if (!System.IO.File.Exists(path))
                throw new Exception("Arquivo não encontrado.");

            var arquivo = "";
            
            using (var objReader = new StreamReader(path))
            {
                arquivo = objReader.ReadToEnd();
                objReader.Close();
            }

            var dadosArquivo = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(arquivo));

            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"FCI_" + idArquivoFci + (retorno ? "_retorno" : "") + ".txt\"");
            
            // Passa o arquivo para download
            byte[] download = new byte[4096];
            int read = 0;
            while ((read = dadosArquivo.Read(download, 0, download.Length)) > 0)
            {
                context.Response.OutputStream.Write(download, 0, read);
                context.Response.Flush();
            }
        }
        catch (Exception ex)
        {
            // Devolve o erro
            context.Response.ContentType = "text/html";
            context.Response.Write(GetErrorResponse(ex));
            return;
        }

        
    }

    private string GetErrorResponse(Exception ex)
    {
        bool debug = false;

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
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}