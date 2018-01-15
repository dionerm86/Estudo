<%@ WebHandler Language="C#" Class="ExportarFerragem" %>

using System.Linq;
using System.Web;

public class ExportarFerragem : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Recupera os ids ferragem informados por URL.
        var idsFerragem = !string.IsNullOrWhiteSpace(context.Request["idsFerragem"]) ?
            context.Request["idsFerragem"].Split(',').Select(f => Glass.Conversoes.StrParaInt(f)).Where(f => f > 0).ToList() : new System.Collections.Generic.List<int>();

        // Não prossegue com a exportação caso nenhum ID de ferragem seja recuperado.
        if (idsFerragem.Count == 0)
            throw new System.Exception("Selecione pelo menos uma ferragem para exportar.");
        
        // Recupera o fluxo de exportação/importação de ferragem.
        var exportacaoImportacaoferragem = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Projeto.Negocios.IExportacaoImportacaoFerragem>();

        // Escreve o arquivo no buffer.
        using (System.IO.MemoryStream arquivo = new System.IO.MemoryStream(exportacaoImportacaoferragem.Exportar(idsFerragem)))
        {
            // Indica que será feito um download do arquivo.
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"Ferragens.wgp\"");
            context.Response.AddHeader("Content-Length", arquivo.Length.ToString());

            // Passa o arquivo para download.
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