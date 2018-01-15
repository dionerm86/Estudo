<%@ WebHandler Language="C#" Class="NFeEntradaTerceirosXML" %>

using System;
using System.Web;
using System.Xml;

public class NFeEntradaTerceirosXML : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        // Recuperação das informações passadas por GET
        uint idNf = Glass.Conversoes.StrParaUint(context.Request.QueryString["idNfTer"]);
        Glass.Data.Model.NotaFiscal nf = Glass.Data.DAL.NotaFiscalDAO.Instance.GetElement(idNf);

        // Verifica se o arquivo da NF-e existe
        var path = Glass.Data.Helper.Utils.GetNfeXmlPath + nf.ChaveAcesso + "-ter.xml";
        if (nf.TipoDocumento != (int)Glass.Data.Model.NotaFiscal.TipoDoc.EntradaTerceiros || !System.IO.File.Exists(path))
            context.Response.Redirect("~/Listas/LstNotaFiscal.aspx?erroNf=" + idNf.ToString());

        // Carrega arquivo da NF-e
        XmlDocument xml = new XmlDocument();
        xml.Load(path);

        // Indica que será feito um download do arquivo
        context.Response.ContentType = "text/xml";
        context.Response.AddHeader("Content-Disposition", "attachment; filename=\"NF-e " + nf.NumeroNFe.ToString("0##,###,###") + ".xml\"");
        context.Response.Write(xml.OuterXml.ToString());
        context.Response.Flush();
    }

    public bool IsReusable
    {
        get { return false; }
    }
}