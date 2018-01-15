<%@ WebHandler Language="C#" Class="ArquivoRemessa" %>

using System;
using System.Web;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.IO;
using System.Linq;

public class ArquivoRemessa : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var arqRemessa = ArquivoRemessaDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(context.Request["id"]));
        var logImportacao = context.Request["logImportacao"] != null && context.Request["logImportacao"].ToLower() == "true";

        if (arqRemessa != null)
        {
            var filePath = logImportacao ? arqRemessa.CaminhoArquivoLog : arqRemessa.CaminhoArquivo;
            var fileName = logImportacao ? arqRemessa.NomeArquivoLog : arqRemessa.NomeArquivo;

            if (!logImportacao)
            {
                var arquivos = new DirectoryInfo(Path.GetDirectoryName(filePath))
                    .GetFiles()
                    .Where(f => f.Name.IndexOf('_') != -1).ToList();

                var arqAntigos = arquivos.Where(f => f.Name.Split('_').Count() == 2).ToArray();
                var arqNovos = arquivos.Where(f => f.Name.Split('_').Count() > 2).ToArray();

                var arquivo = arqAntigos.Where(f => Glass.Conversoes.StrParaUint(f.Name.Substring(0, f.Name.IndexOf('_'))) == arqRemessa.IdArquivoRemessa &&
                        f.Name.Substring(f.Name.IndexOf('_') + 1).ToLower() != "log.txt")
                    .Select(f => new { filePath = f.FullName, fileName = f.Name.Substring(f.Name.IndexOf('_') + 1) }).FirstOrDefault();

                if (arquivo == null)
                {
                    arquivo = arqNovos.Where(f => Glass.Conversoes.StrParaUint(f.Name.Substring(0, f.Name.IndexOf('_'))) == arqRemessa.IdArquivoRemessa &&
                       f.Name.Substring(f.Name.IndexOf('_', f.Name.IndexOf('_') + 1) + 1).ToLower() != "log.txt")
                   .Select(f => new { filePath = f.FullName, fileName = f.Name.Substring(f.Name.IndexOf('_', f.Name.IndexOf('_') + 1) + 1) }).FirstOrDefault();
                }

                if (arquivo != null)
                {
                    filePath = arquivo.filePath;
                    fileName = arquivo.fileName;
                }
            }

            var encoding =
                /* Chamado 38513.
                 * O arquivo será codificado como ANSI caso o banco seja o SICOOB. */
                ContaBancoDAO.Instance.ObtemCodigoBanco(arqRemessa.IdContaBanco).GetValueOrDefault() == 756 ?
                    "1252" : string.Empty;

            context.Response.Redirect("Download.ashx?filePath=" + filePath + "&fileName=" + fileName + "&encoding=" + encoding);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}