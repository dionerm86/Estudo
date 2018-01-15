<%@ WebHandler Language="C#" Class="Download" %>

using System.Web;

public class Download : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Define o arquivo
        string caminhoArquivo = context.Request["filePath"];

        string fileName = context.Request["fileName"];

        if (caminhoArquivo.Contains("../") || caminhoArquivo.Contains("~/"))
            caminhoArquivo = context.Server.MapPath(caminhoArquivo);

        // Escreve o arquivo no buffer
        using (System.IO.Stream arquivo = System.IO.File.OpenRead(caminhoArquivo))
        {
            /* Chamado 38513. */
            if (!string.IsNullOrEmpty(context.Request["encoding"]) && Glass.Conversoes.StrParaIntNullable(context.Request["encoding"]) > 0)
                context.Response.ContentEncoding = System.Text.Encoding.GetEncoding(Glass.Conversoes.StrParaInt(context.Request["encoding"]));
            
            // Indica que será feito um download do arquivo.
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            context.Response.AddHeader("Content-Length", arquivo.Length.ToString());

            // Passa o arquivo para download
            byte[] download = new byte[4096];
            int read = 0;
            while ((read = arquivo.Read(download, 0, download.Length)) > 0)
            {
                context.Response.OutputStream.Write(download, 0, read);
                context.Response.Flush();
            }
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}