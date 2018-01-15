<%@ WebHandler Language="C#" Class="Boleto" %>


using System;
using System.Web;
using WebGlass.Business.Boleto.Fluxo;
using Glass.Data.Helper;
using Glass;

public class Boleto : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var codigoContaBanco = context.Request["codigoContaBanco"].StrParaUint();
        var carteira = context.Request["carteira"];
        var especieDocumento = context.Request["especieDocumento"].StrParaInt();
        var instrucoes = context.Request["instrucoes"].Split(';');

        var codigoNotaFiscal = context.Request["codigoNotaFiscal"].StrParaUint();
        var codigoContaReceber = context.Request["codigoContaReceber"].StrParaUint();
        var codigoLiberacao = context.Request["codigoLiberacao"].StrParaUint();

        var writer = new System.IO.StringWriter();
        var htmlWriter = new System.Web.UI.HtmlTextWriter(writer);

        BoletoNet.BoletoBancario.HtmlOfflineHeader(writer.GetStringBuilder());

        var dadosBoleto = 
                codigoNotaFiscal > 0 ?
                    DadosBoleto.Instance.ObtemDadosBoleto(codigoContaBanco, carteira, especieDocumento, instrucoes, codigoNotaFiscal, htmlWriter) :
                codigoLiberacao > 0 ? 
                    DadosBoleto.Instance.ObtemDadosBoleto(codigoContaBanco, carteira, especieDocumento, instrucoes, htmlWriter, codigoLiberacao) : 
                    DadosBoleto.Instance.ObtemDadosBoleto(codigoContaBanco, carteira, especieDocumento, instrucoes, htmlWriter, new uint[] { codigoContaReceber });

        BoletoNet.BoletoBancario.HtmlOfflineFooter(writer.GetStringBuilder());

        writer.Write("<script> window.print(); </script>");

        htmlWriter.Flush();

        foreach (var b in dadosBoleto)
            Impresso.Instance.IndicarBoletoImpresso((int)b, (int)codigoNotaFiscal, (int)codigoLiberacao, (int)codigoContaBanco,  UserInfo.GetUserInfo);

        context.Response.ContentType = "text/html";
        context.Response.ContentEncoding = System.Text.Encoding.UTF8;
        context.Response.Write(writer.GetStringBuilder().ToString());
    }

    public bool IsReusable
    {
        get { return false; }
    }
}