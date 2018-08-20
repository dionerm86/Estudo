<%@ WebHandler Language="C#" Class="NotaXmlLote" %>

using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Data.Helper;
using Ionic.Utils.Zip;

public class NotaXmlLote : IHttpHandler
{
    public void ProcessRequest (HttpContext context)
    {
        context.Response.ContentType = "application/zip";
        context.Response.AddHeader("content-disposition", "attachment; filename=\"NFEs.zip\"");

        uint numeroNfe = Glass.Conversoes.StrParaUint(context.Request["numeroNfe"]);
        uint idPedido = Glass.Conversoes.StrParaUint(context.Request["idPedido"]);
        uint idLoja = Glass.Conversoes.StrParaUint(context.Request["idLoja"]);
        uint idCliente = Glass.Conversoes.StrParaUint(context.Request["idCliente"]);
        uint idFornec = Glass.Conversoes.StrParaUint(context.Request["idFornec"]);
        int tipoDoc = Glass.Conversoes.StrParaInt(context.Request["tipoDocumento"]);
        //int situacao = Glass.Conversoes.StrParaInt(context.Request["situacao"]);
        string situacao = context.Request["situacao"];
        int formaEmissao = Glass.Conversoes.StrParaInt(context.Request["formaEmissao"]);
        string tipo = context.Request["tipo"];

        var notas = NotaFiscalDAO.Instance.GetListPorSituacao(
            numeroNfe,
            idPedido,
            context.Request["modelo"],
            idLoja,
            idCliente,
            context.Request["nomeCliente"],
            Glass.Conversoes.StrParaInt(context.Request["tipoFiscal"]),
            idFornec,
            context.Request["nomeFornec"],
            context.Request["codRota"],
            tipoDoc,
            situacao,
            context.Request["dataIni"],
            context.Request["dataFim"],
            context.Request["idsCfop"],
            context.Request["tiposCfop"],
            context.Request["dataEntSaiIni"],
            context.Request["dataEntSaiFim"],
            Glass.Conversoes.StrParaUint(context.Request["formaPagto"]),
            context.Request["idsFormaPagtoNotaFiscal"],
            Glass.Conversoes.StrParaInt(context.Request["tipoNf"]),
            Glass.Conversoes.StrParaInt(context.Request["finalidade"]),
            formaEmissao,
            context.Request["infCompl"],
            context.Request["codInternoProd"],
            context.Request["descrProd"],
            context.Request["valorInicial"],
            context.Request["valorFinal"], null, null,
            Glass.Conversoes.StrParaInt(context.Request["ordenar"]),
            null,
            0,
            int.MaxValue);

        using (ZipFile zip = new ZipFile(context.Response.OutputStream))
        {
            foreach (NotaFiscal nf in notas)
            {
                string path = string.Empty;

                if (tipo == "inut")
                {
                    path = Utils.GetNfeXmlPath + nf.IdNf + "-Inut.xml";
                }
                else
                {
                    path = Utils.GetNfeXmlPath + nf.ChaveAcesso + "-nfe.xml";

                    if (string.IsNullOrEmpty(nf.ChaveAcesso))
                        continue;
                }

                if (!File.Exists(path))
                    continue;

                // Verifica se existe xml de cancelamento da nota
                var pathNotaCanc = Utils.GetNfeXmlPath + "110111" + nf.ChaveAcesso + "-can.xml";
                var baixarXmlCanc = nf.Situacao == (int)NotaFiscal.SituacaoEnum.Cancelada && File.Exists(pathNotaCanc);

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
                    nfeProc.SetAttribute("versao", xml["NFe"]["infNFe"].Attributes["versao"].InnerText);

                    // Recria a NFe
                    XmlElement NFe = xml.CreateElement("NFe");
                    NFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
                    NFe.InnerXml = xml["NFe"].InnerXml;
                    nfeProc.AppendChild(NFe);

                    // Recria o Protocolo de Autorização
                    XmlElement protNFe = xml.CreateElement("protNFe");
                    protNFe.SetAttribute("versao", xml["NFe"]["infNFe"].Attributes["versao"].InnerText);
                    protNFe.InnerXml = xmlProt;
                    nfeProc.AppendChild(protNFe);

                    XmlDocument xml1 = new XmlDocument();
                    xml1.LoadXml(nfeProc.OuterXml);

                    if (baixarXmlCanc)
                        zip.AddFile(pathNotaCanc, "");

                    zip.AddStringAsFile(xml1.OuterXml, nf.ChaveAcesso + "-nfe.xml", "");
                }
                else if (System.IO.File.Exists(path))
                {
                    zip.AddStringAsFile(xml.OuterXml, nf.NumeroNFe + ".xml", "");
                }
            }

            zip.Save();
        }

    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}