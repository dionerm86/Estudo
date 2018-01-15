<%@ WebHandler Language="C#" Class="CteXml" %>

using System;
using System.Web;
using System.Xml;

public class CteXml : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Recuperação das informações passadas por GET
        uint idCte = Glass.Conversoes.StrParaUint(context.Request.QueryString["idCte"]);
        Glass.Data.Model.Cte.ConhecimentoTransporte cte = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetElement(idCte);

        // Verifica se o arquivo do CT-e existe
        string path = Glass.Data.Helper.Utils.GetCteXmlPath + cte.ChaveAcesso + "-cte.xml";
        if (!System.IO.File.Exists(path))
            context.Response.Redirect("~/Listas/LstConhecimentoTransporte.aspx?erroCte=" + idCte.ToString());

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

            // Recria a CTe
            XmlElement CTe = xml.CreateElement("CTe");
            CTe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
            CTe.InnerXml = xml["CTe"].InnerXml;
            cteProc.AppendChild(CTe);

            // Recria o Protocolo de Autorização
            XmlElement protCTe = xml.CreateElement("protCTe");
            protCTe.SetAttribute("versao", Glass.Data.CTeUtils.ConfigCTe.VersaoCte);
            protCTe.InnerXml = xmlProt;
            cteProc.AppendChild(protCTe);

            // Indica que será feito um download do arquivo
            context.Response.ContentType = "text/xml";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"CT-e " + cte.NumeroCte.ToString("0##,###,###") + ".xml\"");

            XmlDocument xml1 = new XmlDocument();
            xml1.LoadXml(cteProc.OuterXml);

            context.Response.Write(xml1.OuterXml.ToString());
            context.Response.Flush();
        }
        else
        {
            // Indica que será feito um download do arquivo
            context.Response.ContentType = "text/xml";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"CT-e " + cte.NumeroCte.ToString("0##,###,###") + ".xml\"");
            context.Response.Write(xml.OuterXml.ToString());
            context.Response.Flush();
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}