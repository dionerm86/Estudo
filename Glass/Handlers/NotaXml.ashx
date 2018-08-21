<%@ WebHandler Language="C#" Class="NotaXml" %>

using System;
using System.Web;
using System.Xml;
using Ionic.Utils.Zip;
using System.IO;
using Glass.Data.Helper;

public class NotaXml : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Recuperação das informações passadas por GET
        uint idNf = Glass.Conversoes.StrParaUint(context.Request.QueryString["idNf"]);
        Glass.Data.Model.NotaFiscal nf = Glass.Data.DAL.NotaFiscalDAO.Instance.GetElement(idNf);
        var tipo = context.Request.QueryString["tipo"];

        // Verifica se existe xml de cancelamento da nota
        var pathNotaCanc = Glass.Data.Helper.Utils.GetNfeXmlPath + "110111" + nf.ChaveAcesso + "-can.xml";
        var baixarXmlCanc = nf.Situacao == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Cancelada &&
            System.IO.File.Exists(pathNotaCanc);

        // Verifica se o arquivo da NF-e existe
        var path = string.Empty;

        if(tipo == "inut")
        {
            path = Utils.GetNfeXmlPath + idNf + "-Inut.xml"; 
        }
        else
        {
            path = Utils.GetNfeXmlPath + nf.ChaveAcesso + "-nfe.xml";
        }

        if (nf.Situacao == (int)Glass.Data.Model.NotaFiscal.SituacaoEnum.Autorizada && !System.IO.File.Exists(path))
            context.Response.Redirect("~/Listas/LstNotaFiscal.aspx?erroNf=" + idNf.ToString());

        // Carrega arquivo da NF-e
        XmlDocument xml = new XmlDocument();
        xml.Load(path);

        // Põe o protocolo de autorização na posição correta, caso a nota já não esteja correta
        if (xml["NFe"] != null && xml["NFe"]["infProt"] != null)
        {
            // Salva o protocolo de autorização em uma variável
            string xmlProt = xml["NFe"]["infProt"].OuterXml;

            // Remove o protocolo de autorização de dentro da NFe
            xml["NFe"].RemoveChild(xml["NFe"]["infProt"]);

            // Cria container para inserir a NF-e e o protocolo de autorização
            XmlElement nfeProc = xml.CreateElement("nfeProc");
            nfeProc.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            nfeProc.SetAttribute("versao", Glass.Data.NFeUtils.ConfigNFe.VersaoNFe);

            // Recria a NFe
            XmlElement NFe = xml.CreateElement("NFe");
            NFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            NFe.InnerXml = xml["NFe"].InnerXml;
            nfeProc.AppendChild(NFe);

            // Recria o Protocolo de Autorização
            XmlElement protNFe = xml.CreateElement("protNFe");
            protNFe.SetAttribute("versao", Glass.Data.NFeUtils.ConfigNFe.VersaoNFe);
            protNFe.InnerXml = xmlProt;
            nfeProc.AppendChild(protNFe);

            XmlDocument xml1 = new XmlDocument();
            xml1.LoadXml(nfeProc.OuterXml);

            if (baixarXmlCanc)
            {
                // Adiciona o arquivo de otimização ao zip            
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nf.NumeroNFe + ".zip\"");

                var aux = new Action<System.IO.Stream>(stream =>
                {
                    // Adiciona os arquivos SAG
                    using (ZipFile zip = new ZipFile(stream))
                    {
                        zip.AddFile(pathNotaCanc, "");
                        zip.AddStringAsFile(xml1.OuterXml, nf.ChaveAcesso + "-nfe.xml", null);

                        zip.Save();
                    }
                });

                using (var arq = System.IO.File.Create(Utils.GetArquivoOtimizacaoPath + nf.NumeroNFe + ".zip"))
                {
                    aux(arq);
                    arq.Flush();
                }

                aux(context.Response.OutputStream);
            }
            else
            {
                // Indica que será feito um download do arquivo
                context.Response.ContentType = "text/xml";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"NF-e " + nf.NumeroNFe.ToString("0##,###,###") + ".xml\"");
                context.Response.Write(xml1.OuterXml.ToString());
                context.Response.Flush();
            }
        }
        else
        {
            if (baixarXmlCanc)
            {
                // Adiciona o arquivo de otimização ao zip            
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nf.NumeroNFe + ".zip\"");

                var aux = new Action<System.IO.Stream>(stream =>
                {
                    // Adiciona os arquivos SAG
                    using (ZipFile zip = new ZipFile(stream))
                    {
                        zip.AddFile(pathNotaCanc);
                        zip.AddFile(path);

                        zip.Save();
                    }
                });

                using (var arq = System.IO.File.Create(Utils.GetArquivoOtimizacaoPath + nf.NumeroNFe + ".zip"))
                {
                    aux(arq);
                    arq.Flush();
                }

                aux(context.Response.OutputStream);
            }
            else
            {
                // Indica que será feito um download do arquivo
                context.Response.ContentType = "text/xml";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"NF-e " + nf.NumeroNFe.ToString("0##,###,###") + ".xml\"");
                context.Response.Write(xml.OuterXml.ToString());
                context.Response.Flush();
            }
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}