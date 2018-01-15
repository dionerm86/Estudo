<%@ WebHandler Language="C#" Class="ExportarProjeto" %>

using System;
using System.Web;
using Glass.Data.Helper;

public class ExportarProjeto : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Escreve o arquivo no buffer
        using (System.IO.MemoryStream arquivo = new System.IO.MemoryStream(
            UtilsExportacaoProjeto.Exportar(context.Request["idsProjetoModelo"], context.Request["semFolgas"] == "true")))
        {
            // Indica que será feito um download do arquivo
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"Projetos.wgp\"");
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