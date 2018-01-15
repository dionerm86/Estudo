<%@ WebHandler Language="C#" Class="BackupDB" %>

using System;
using System.Web;

public class BackupDB : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Escreve o arquivo no buffer
        using (System.IO.Stream arquivo = new Glass.BackupBancoDados().BackupMySQLDatabase())
        {
            // Indica que será feito um download do arquivo
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"Backup " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + ".zip\"");

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