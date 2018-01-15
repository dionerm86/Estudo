<%@ WebHandler Language="C#" Class="ValidadorCalcEngine" %>

using System;
using System.Web;
using System.Xml;

public class ValidadorCalcEngine : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var dadosRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(Glass.Data.DAL.ArquivoMesaCorteDAO.Instance.ValidarArquivoCalcEngine(context.Request["nome"]));

        context.Response.ContentType = "application/json";
        context.Response.Write(dadosRetorno);
    }

    public bool IsReusable
    {
        get { return false; }
    }
}