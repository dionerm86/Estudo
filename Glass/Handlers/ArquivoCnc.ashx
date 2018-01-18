<%@ WebHandler Language="C#" Class="ArquivoCnc" %>

using System;
using System.Web;
using Ionic.Utils.Zip;
using Glass.Data.Helper;
using System.IO;
using NPOI.HSSF.UserModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;
using System.Linq;

//public class Dados
//{
//    public Dados()
//    {
//        DadosEtiqueta = new List<Etiqueta>();
//    }
//    public List<Etiqueta> DadosEtiqueta { get; set; }

//    public int TamMaxEtiqueta { get { return DadosEtiqueta.Select(f => f.NumEtiqueta.Length).Max(); } }

//    public int TamMaxAltura { get { return DadosEtiqueta.Select(f => f.Altura.ToString().Length).Max(); } }

//    public int TamMaxLargura { get { return DadosEtiqueta.Select(f => f.Largura.ToString().Length).Max(); } }
    
//    public class Etiqueta
//    {
//        public string NumEtiqueta { get; set; }
//        public float Altura { get; set; }
//        public int Largura { get; set; }
//        public float Espessura { get; set; }
//    }
//}

public class ArquivoCnc : IHttpHandler 
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var data = DateTime.Now;

            //Nome do arquivo que sera salvo
            var nomeArquivo = "CNC_" + data.ToShortDateString() + "_" + data.ToShortTimeString() + "_" + data.Millisecond;

            //Cria o arquivo xls
            var workbook = new HSSFWorkbook();
            var worksheet = workbook.CreateSheet("CNC");

            //Monta o cabeçalho do arquivo xls
            CriarCabecalhoExcel(worksheet);

            //Variavel para aramazenar os dados do arquivo txt
            var txtFile = new List<string>();

            //Busca os produtos do pedido espelho
            var lstProdPedEsp = ProdutosPedidoEspelhoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(context.Request["idPedido"]),
                Glass.Conversoes.StrParaUint(context.Request["idCliente"]), context.Request["NomeCliente"],
                Glass.Conversoes.StrParaUint(context.Request["idLoja"]), Glass.Conversoes.StrParaUint(context.Request["idFunc"]),
                Glass.Conversoes.StrParaUint(context.Request["idFuncionarioConferente"]), Glass.Conversoes.StrParaInt(context.Request["situacao"]),
                context.Request["situacaoPedOri"], context.Request["idsProcesso"], context.Request["dataIniEnt"], context.Request["dataFimEnt"],
                context.Request["dataIniFab"], context.Request["dataFimFab"], context.Request["dataIniFin"], context.Request["dataFimFin"],
                context.Request["dataIniConf"], context.Request["dataFimConf"], false, context.Request["pedidosSemAnexos"] == "true",
                context.Request["pedidosAComprar"] == "true", context.Request["pedidos"], ((int)PedidoEspelho.SituacaoCncEnum.Projetado).ToString(),
                context.Request["dataIniSituacaoCnc"], context.Request["dataFimSituacaoCnc"], context.Request["tipoPedido"], context.Request["idsRotas"],
                Glass.Conversoes.StrParaInt(context.Request["origemPedido"]), Glass.Conversoes.StrParaInt(context.Request["pedidosConferidos"]));

            //var dados = new Dados();

            //Percorre os produtos
            foreach (var prodPedEsp in lstProdPedEsp)
            {
                if (ProdutosPedidoEspelhoDAO.Instance.DeveGerarProjCNC(prodPedEsp))
                {

                    //Busca as etiquetas da peça
                    var etqs = PecaItemProjetoDAO.Instance.ObtemEtiquetas(prodPedEsp.IdPedido, prodPedEsp.IdProdPed, Convert.ToInt32(prodPedEsp.Qtde));

                    foreach (var etq in etqs.Split(','))
                    {
                        //Gera o registro no arquivo xls
                        CriaLinhaExcel(etq, prodPedEsp.Espessura, worksheet);

                        //Gera o registro no arquivo txt
                        CriaLinhaArquivoTxt(etq, prodPedEsp.Altura, prodPedEsp.Largura, prodPedEsp.Espessura, txtFile/*, dados.TamMaxAltura,
                            dados.TamMaxLargura, dados.TamMaxEtiqueta*/
                                                                           );
                    }

                    //var dadosEtq = new Dados.Etiqueta();

                    ////Percorre as etiquetas da peça
                    //foreach (var etq in etqs.Split(','))
                    //{
                    //    dadosEtq.NumEtiqueta = etq;
                    //    dadosEtq.Altura = prodPedEsp.Altura;
                    //    dadosEtq.Largura = prodPedEsp.Largura;
                    //    dadosEtq.Espessura = prodPedEsp.Espessura;
                    //}

                    //dados.DadosEtiqueta.Add(dadosEtq);

                }
            }

            //foreach (var dadoEtq in dados.DadosEtiqueta)
            //{
            //    //Gera o registro no arquivo xls
            //    CriaLinhaExcel(dadoEtq.NumEtiqueta, dadoEtq.Espessura, worksheet);

            //    //Gera o registro no arquivo txt
            //    CriaLinhaArquivoTxt(dadoEtq.NumEtiqueta, dadoEtq.Altura, dadoEtq.Largura, dadoEtq.Espessura, txtFile/*, dados.TamMaxAltura,
            //        dados.TamMaxLargura, dados.TamMaxEtiqueta*/);
            //}

            context.Response.ContentType = "application/zip";
            context.Response.AddHeader("content-disposition", "attachment; filename=\"" + nomeArquivo + ".zip\"");

            //Zipa os arquivos
            using (var zip = new ZipFile(context.Response.OutputStream))
            {
                //Gera o arquivo xls(Excel)
                var msXls = new MemoryStream();
                workbook.Write(msXls);

                //zipa o xls
                zip.AddFileStream(nomeArquivo + ".xls", "", msXls);

                //Gera o arquivo txt
                var msTxt = new MemoryStream();
                TextWriter tw = new StreamWriter(msTxt, System.Text.Encoding.UTF8);

                foreach (var str in txtFile)
                    tw.Write(str + Environment.NewLine);

                tw.Flush();

                //zipa o txt
                zip.AddFileStream(nomeArquivo + ".txt", "", msTxt);

                //Salva o arquivo zip
                zip.Save();

                msXls.Close();
                msTxt.Close();
                tw.Close();

            }
        }
        catch (Exception ex)
        {
            // Devolve o erro
            context.Response.ContentType = "text/html";
            context.Response.Write(GetErrorResponse(ex));
            return;
        }
    }

    private string GetErrorResponse(Exception ex)
    {
        bool debug = false;

        string html = debug ? ex.ToString().Replace("\n", "<br>").Replace("\r", "").Replace(" ", "&nbsp;") : @"
            <script type='text/javascript'>
                alert('" + ex.Message + (ex.InnerException != null ? " " + ex.InnerException.Message : "") + @"');
                window.history.go(-1);
            </script>";

        return @"
            <html>
                <body>
                    " + html + @"
                </body>
            </html>";
    }
 
    public bool IsReusable 
    {
        get 
        {
            return false;
        }
    }

    /// <summary>
    /// Cria o cabeçalho o xls.
    /// </summary>
    /// <param name="worksheet"></param>
    public void CriarCabecalhoExcel(NPOI.SS.UserModel.ISheet worksheet)
    {
        var rowHeader = worksheet.CreateRow(0);

        rowHeader.CreateCell(0).SetCellValue("Barcode");
        rowHeader.CreateCell(1).SetCellValue("File Dxf");
        rowHeader.CreateCell(2).SetCellValue("Dim.Ori");
        rowHeader.CreateCell(3).SetCellValue("Dim.Vert");
        rowHeader.CreateCell(4).SetCellValue("Spess.");
        rowHeader.CreateCell(5).SetCellValue("Area");
        rowHeader.CreateCell(6).SetCellValue("Info");
        rowHeader.CreateCell(7).SetCellValue("Cam Auto VERTEC_6437_SX");
        rowHeader.CreateCell(8).SetCellValue("");
        rowHeader.CreateCell(9).SetCellValue("Ripetizioni");
        rowHeader.CreateCell(10).SetCellValue("ID");
    }

    /// <summary>
    /// Cria um registro no xls.
    /// </summary>
    /// <param name="etiqueta"></param>
    /// <param name="espessura"></param>
    /// <param name="worksheet"></param>
    public void CriaLinhaExcel(string etiqueta, float espessura, NPOI.SS.UserModel.ISheet worksheet)
    {
        if (worksheet == null)
            return;

        var row = worksheet.CreateRow(worksheet.LastRowNum + 1);

        row.CreateCell(0).SetCellValue(etiqueta.Replace(" ", "").Trim());
        row.CreateCell(1).SetCellValue(etiqueta.Replace(".", "").Replace("/", "").Trim());
        row.CreateCell(2).SetCellValue("");
        row.CreateCell(3).SetCellValue("");
        row.CreateCell(4).SetCellValue(espessura);
        row.CreateCell(5).SetCellValue("");
        row.CreateCell(6).SetCellValue("");
        row.CreateCell(7).SetCellValue("VIDRO_" + espessura);
        row.CreateCell(8).SetCellValue("");
        row.CreateCell(9).SetCellValue("");
        row.CreateCell(10).SetCellValue("");
    }

    /// <summary>
    /// Cria um registro no arquivo txt.
    /// </summary>
    /// <param name="etiqueta"></param>
    /// <param name="altura"></param>
    /// <param name="largura"></param>
    /// <param name="espessura"></param>
    /// <param name="txt"></param>
    /// <param name="tamMaxAltura"></param>
    /// <param name="tamMaxLargura"></param>
    /// <param name="tamMaxEtiqueta"></param>
    public void CriaLinhaArquivoTxt(string etiqueta, float altura, int largura, float espessura, List<string> txt/*,
        int tamMaxAltura, int tamMaxLargura, int tamMaxEtiqueta*/)
    {
        etiqueta = etiqueta.Replace(" ", "").Trim();

        var dadosEtiqueta = etiqueta.Split('-');

        string linha = etiqueta + " ";//+ InsereEspaco(tamMaxEtiqueta - etiqueta.Length + 3);

        linha += (dadosEtiqueta[0].Trim() + "-" + dadosEtiqueta[1].Replace(".", "").Replace("/", "") + ".dxf") +
            " ";//InsereEspaco(tamMaxEtiqueta - etiqueta.Length + 3);

        if (altura >= largura)
        {
            linha += altura.ToString() + " "; //InsereEspaco(tamMaxAltura - altura.ToString().Length + 3);
            linha += largura.ToString() + " "; //InsereEspaco(tamMaxLargura - largura.ToString().Length + 3);
        }
        else
        {
            linha += largura.ToString() + " "; //InsereEspaco(tamMaxLargura - largura.ToString().Length + 3);
            linha += altura.ToString() + " "; //InsereEspaco(tamMaxAltura - altura.ToString().Length + 3);
        }

        linha += espessura;

        txt.Add(linha);
    }

    private string InsereEspaco(int qtde)
    {
        return "".PadLeft(qtde, ' ');
    }
}