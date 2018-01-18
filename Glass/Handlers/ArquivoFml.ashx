<%@ WebHandler Language="C#" Class="ArquivoFml" %>

using System;
using System.Web;
using Ionic.Utils.Zip;
using Glass.Data.Helper;
using System.IO;
using NPOI.HSSF.UserModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

public class ArquivoFml : IHttpHandler 
{
    public void ProcessRequest(HttpContext context)
    {
        var nomeArquivo = "Arquivo FML " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");
        var idPedido = Glass.Conversoes.StrParaUint(context.Request["idPedido"]);

        //Busca os produtos do pedido espelho
        var lstProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(context.Request["idPedido"]),
            Glass.Conversoes.StrParaUint(context.Request["idCliente"]), context.Request["NomeCliente"], Glass.Conversoes.StrParaUint(context.Request["idLoja"]),
            Glass.Conversoes.StrParaUint(context.Request["idFunc"]), Glass.Conversoes.StrParaUint(context.Request["idFuncionarioConferente"]),
            Glass.Conversoes.StrParaInt(context.Request["situacao"]), context.Request["situacaoPedOri"], context.Request["idsProcesso"],
            context.Request["dataIniEnt"], context.Request["dataFimEnt"], context.Request["dataIniFab"], context.Request["dataFimFab"],
            context.Request["dataIniFin"], context.Request["dataFimFin"], context.Request["dataIniConf"], context.Request["dataFimConf"],
            false, context.Request["pedidosSemAnexos"] == "true", context.Request["pedidosAComprar"] == "true", context.Request["pedidos"],
            null, null, null, context.Request["tipoPedido"], context.Request["idsRotas"], Glass.Conversoes.StrParaInt(context.Request["origemPedido"]), Glass.Conversoes.StrParaInt(context.Request["pedidosConferidos"]));

        var resultado = PedidoEspelhoDAO.Instance.GerarArquivoFmlPeloPedido(lstProdPedEsp, false);

        // Adiciona o arquivo de otimização ao zip
        context.Response.ContentType = "application/zip";
        context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

        resultado.Compactar(context.Response.OutputStream);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}