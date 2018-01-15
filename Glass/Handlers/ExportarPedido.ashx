<%@ WebHandler Language="C#" Class="ExportarPedido" %>

using System;
using System.Web;
using Glass.Data.Helper;

public class ExportarPedido : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        //// Escreve o arquivo no buffer
        //using (System.IO.MemoryStream arquivo = new System.IO.MemoryStream(UtilsExportacaoPedido.Exportar(context.Request["idsPedido"], context.Request["idsProdutos"])))
        //{
        //    // Indica que será feito um download do arquivo
        //    context.Response.ContentType = "application/octet-stream";
        //    context.Response.AddHeader("Content-Disposition", "attachment; filename=\"Pedidos.wge\"");
        //    context.Response.AddHeader("Content-Length", arquivo.Length.ToString());

        //    // Passa o arquivo para download
        //    byte[] download = new byte[4096];
        //    int read = 0;
        //    while ((read = arquivo.Read(download, 0, download.Length)) > 0)
        //    {
        //        context.Response.OutputStream.Write(download, 0, read);
        //        context.Response.Flush();
        //    }
        //}
    }

    public bool IsReusable
    {
        get { return false; }
    }
}