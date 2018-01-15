<%@ WebHandler Language="C#" Class="CteXmlLote" %>

using System.Web;
using System.IO;
using System.Xml;
using Ionic.Utils.Zip;

public class CteXmlLote : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/zip";
        context.Response.AddHeader("content-disposition", "attachment; filename=\"CTEs.zip\"");

        // Recuperação das informações passadas por GET
        var ctes = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetList(
            Glass.Conversoes.StrParaIntNullable(context.Request["numCte"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaIntNullable(context.Request["idLoja"]).GetValueOrDefault(),
            context.Request["situacao"],
            Glass.Conversoes.StrParaUintNullable(context.Request["cfop"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaIntNullable(context.Request["formaPagto"]),
            Glass.Conversoes.StrParaIntNullable(context.Request["tipoEmissao"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaIntNullable(context.Request["tipoCte"]),
            Glass.Conversoes.StrParaIntNullable(context.Request["tipoServico"]),
            context.Request["dataIni"],
            context.Request["dataFim"],
            Glass.Conversoes.StrParaUintNullable(context.Request["idTransportador"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaIntNullable(context.Request["ordenar"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["tipoRemetente"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["idRemetente"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["tipoDestinatario"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["idDestinatario"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["tipoRecebedor"]).GetValueOrDefault(),
            Glass.Conversoes.StrParaUintNullable(context.Request["idRecebedor"]).GetValueOrDefault(),
            string.Empty, 0, 0);

        using (ZipFile zip = new ZipFile(context.Response.OutputStream))
        {
            foreach (var cte in ctes)
            {
                if (string.IsNullOrEmpty(cte.ChaveAcesso))
                    continue;
                
                var path = Glass.Data.Helper.Utils.GetCteXmlPath + cte.ChaveAcesso + "-cte.xml";

                if (!File.Exists(path))
                    continue;
                
                // Carrega arquivo do CT-e
                XmlDocument xml = new XmlDocument();
                xml.Load(path);

                // Põe o protocolo de autorização na posição correta, caso a nota já não esteja correta
                if (xml["CTe"] != null && xml["CTe"]["infProt"] != null)
                {
                    // Salva o protocolo de autorização em uma variável
                    string xmlProt = xml["CTe"]["infProt"].OuterXml;
                        
                    // Remove o protocolo de autorização de dentro do CTe
                    xml["CTe"].RemoveChild(xml["CTe"]["infProt"]);
                        
                    // Cria container para inserir o CT-e e o protocolo de autorização
                    XmlElement cteProc = xml.CreateElement("cteProc");
                    cteProc.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
                    cteProc.SetAttribute("versao", Glass.Data.CTeUtils.ConfigCTe.VersaoCte);
                        
                    // Recria o CTe
                    XmlElement CTe = xml.CreateElement("CTe");
                    CTe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
                    CTe.InnerXml = xml["CTe"].InnerXml;
                    cteProc.AppendChild(CTe);
                        
                    // Recria o Protocolo de Autorização
                    XmlElement protCTe = xml.CreateElement("protCTe");
                    protCTe.SetAttribute("versao", Glass.Data.CTeUtils.ConfigCTe.VersaoCte);
                    protCTe.InnerXml = xmlProt;
                    cteProc.AppendChild(protCTe);

                    XmlDocument xml1 = new XmlDocument();
                    xml1.LoadXml(cteProc.OuterXml);

                    zip.AddStringAsFile(xml1.OuterXml, cte.ChaveAcesso + "-cte.xml", "");
                }
                else if (File.Exists(path))
                {
                    zip.AddFile(path, "");
                }
            }

            zip.Save();
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}