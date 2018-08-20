<%@ WebHandler Language="C#" Class="ArquivoOtimizacao" %>

using System;
using System.Web;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Collections.Generic;
using System.IO;
using Ionic.Utils.Zip;
using System.Linq;

public class ArquivoOtimizacao : IHttpHandler
{
    public void ProcessRequest (HttpContext context)
    {
        if (Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.Nenhum)
        {
            context.Response.End();
            return;
        }

        if (UserInfo.GetUserInfo == null || UserInfo.GetUserInfo.CodUser == 0)
        {
            context.Response.Write("<script>alert(\"Não existe autenticação ativa para concluir essa operação!\"); window.history.back(); </script>");
            context.Response.Flush();
            return;
        }

        // Define que apenas o Arquivo de Mesa será gerado
        bool apenasArqMesa = context.Request["apenasArqMesa"] == "true";
        var arquivoECutter = context.Request["ecutter"] == "true" || Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.eCutter;

        string material = !String.IsNullOrEmpty(context.Request["material"]) ?
            " " + context.Request["material"].Replace("/", "") : String.Empty;

        string extensaoArquivo = Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.CorteCerto ? ".txt" : ".ASC";

        string nomeArquivo = "Arquivo Otimização " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + material;

        string arquivo = null;
        uint idImpressao = context.Request["IdImpressao"] != null ? Glass.Conversoes.StrParaUint(context.Request["IdImpressao"]) : 0;
        var ignorarExportadas = context.Request["ignorarExportadas"] != null ? bool.Parse(context.Request["ignorarExportadas"]) : false;
        var ignorarSag = context.Request["ignorarSag"] != null ? bool.Parse(context.Request["ignorarSag"]) : false;

        List<Etiqueta> lstEtiqueta = new List<Etiqueta>();
        List<byte[]> lstArqMesa = new List<byte[]>(); // Arquivos para mesa de corte
        List<string> lstCodArq = new List<string>(); // Código dos arquivos para mesa de corte
        var lstErrosArq = new List<KeyValuePair<string, Exception>>(); // Erros ao gerar os arquivos
        var errosGeracaoMarcacao = string.Empty;

        // Recupera apenas o arquivo de mesa
        if (apenasArqMesa)
        {
            uint idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPedProducao(context.Request["numEtiqueta"]).GetValueOrDefault(0));
            ProdutosPedidoEspelho prodPed = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(null, idProdPed, true);

            Etiqueta etiq = new Etiqueta();
            etiq.IdProdPedEsp = prodPed.IdProdPed;
            etiq.IdPedido = prodPed.IdPedido.ToString();
            etiq.NumEtiqueta = context.Request["numEtiqueta"];
            // etiq.Forma = prodPed.Forma;
            lstEtiqueta.Add(etiq);

            ImpressaoEtiquetaDAO.Instance.MontaArquivoMesaOptyway(lstEtiqueta, lstArqMesa, lstCodArq, lstErrosArq, Glass.Conversoes.StrParaUint(context.Request["idSetor"]), true, false);
        }
        // Recupera as etiquetas e os arquivos de mesa
        else if (Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.OptyWay ||
                 Glass.Configuracoes.EtiquetaConfig.TipoExportacaoEtiqueta == DataSources.TipoExportacaoEtiquetaEnum.eCutter)
        {
            try
            {
                arquivo = ImpressaoEtiquetaDAO.Instance.ArquivoOtimizacaoOptyWay(UserInfo.GetUserInfo.CodUser, idImpressao, context.Request["etiquetas"],
                    ref lstEtiqueta, ref lstArqMesa, ref lstCodArq, ref lstErrosArq, ignorarExportadas, ignorarSag);
            }
            catch (Exception x)
            {
                context.Response.Write(string.Format("<script>alert(\"{0}\"); window.history.back(); </script>", x.Message));
                context.Response.Flush();
                return;
            }
        }
        else
            arquivo = ImpressaoEtiquetaDAO.Instance.ArquivoOtimizacaoCorteCerto(UserInfo.GetUserInfo.CodUser, idImpressao, context.Request["etiquetas"], ignorarExportadas, ref lstEtiqueta);

        Glass.Data.Model.ArquivoOtimizacao a = ArquivoOtimizacaoDAO.Instance.InserirArquivoOtimizacao(
            Glass.Data.Model.ArquivoOtimizacao.DirecaoEnum.Exportar, (lstArqMesa.Count != 0 || apenasArqMesa) ? ".zip" : extensaoArquivo, lstEtiqueta, lstCodArq);

        a.ExtensaoArquivo = extensaoArquivo;

        if (lstErrosArq.Any())
        {
            var erros = string.Join("</br>", lstErrosArq.Where(f => !string.IsNullOrWhiteSpace(f.Key))
                .Select(f => string.Format("Etiqueta: {0} Erro: {1}.", f.Key, Glass.MensagemAlerta.FormatErrorMsg(null, f.Value))));

            context.Response.Write(string.Format("Situações com arquivos de mesa: </br></br>{0}", erros));
            context.Response.Flush();
            return;
        }

        if (!apenasArqMesa)
        {
            // Salva o arquivo na pasta
            using (FileStream f = File.Create(Utils.GetArquivoOtimizacaoPath + a.NomeArquivo))
            {
                using (StreamWriter w = new StreamWriter(f))
                    w.Write(arquivo);
            }
        }

        // Se não houver arquivos de mesa de corte, salva sem zipar
        if (lstArqMesa.Count == 0 && string.IsNullOrEmpty(errosGeracaoMarcacao) && !apenasArqMesa && !arquivoECutter)
        {
            // Indica que será feito um download do arquivo
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + nomeArquivo + extensaoArquivo + "\"");
            context.Response.OutputStream.Write(System.Text.Encoding.Default.GetBytes(arquivo), 0, arquivo.Length);
            context.Response.Flush();
        }
        else
        {
            // Adiciona o arquivo de otimização ao zip            
            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

            var aux = new Action<System.IO.Stream>(stream =>
            {
                // Adiciona os arquivos SAG
                using (ZipFile zip = new ZipFile(stream))
                {
                    if (!apenasArqMesa)
                        zip.AddFile(Utils.GetArquivoOtimizacaoPath + a.NomeArquivo, "");

                    for (var i = 0; i < lstArqMesa.Count; i++)
                    {
                        /* Chamado 23063. */
                        try
                        {
                            zip.AddFileStream(lstCodArq[i], string.Empty, new MemoryStream(lstArqMesa[i]));
                        }
                        catch
                        {
                            // Ignora esse falha
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(errosGeracaoMarcacao))
                        zip.AddStringAsFile(errosGeracaoMarcacao, "Situações com arquivos de mesa.error", string.Empty);

                    zip.Save();
                }
            });

            using (var arq = System.IO.File.Create(Utils.GetArquivoOtimizacaoPath + (a.NomeArquivo.Substring(0, a.NomeArquivo.Length - 4) + ".zip")))
            {
                aux(arq);
                arq.Flush();
            }

            if (arquivoECutter)
            {
                var url = context.Request.Url;
                var address = url.AbsoluteUri;
                // Altera o protocolo do endereço
                address = string.Format("ecutter-opt{0}", address.Substring(url.Scheme.Length));
                // Remove o nome da página da requisiaç
                address = address.Substring(0,  address.IndexOf("arquivootimizacao.ashx", 0, StringComparison.InvariantCultureIgnoreCase));

                var token = "";
                var authCookie = context.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];

                if (authCookie != null)
                    token = authCookie.Value;

                // Monta o endereço redirecionando para o servido do ecutter
                address = string.Format("{0}ecutteroptimizationservice.ashx?id={1}&token={2}", address, a.IdArquivoOtimizacao, token);

                context.Response.Redirect(address);
            }
            else
                aux(context.Response.OutputStream);
        }
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}