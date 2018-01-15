<%@ WebHandler Language="C#" Class="FaturamentoCarregamento" %>

using System.Web;
using System.IO;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Net;

public class FaturamentoCarregamento : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var itens = (context.Request["idsImprimir"].Replace("(", "").Replace(")", "")).Split(';').ToArray();
        context.Response.ContentType = "application/pdf";

        using (var outPdf = new PdfDocument())
        {
            using (var arquivosConcatenados = new MemoryStream())
            {
                foreach (var item in itens)
                {
                    var bytesLib = Glass.Conversoes.StrParaUint(item.Split(',')[0]) > 0 ? GetBytesRelatorioLib(context, Glass.Conversoes.StrParaUint(item.Split(',')[0])) : new byte[] { };
                    var bytesNf = Glass.Data.Handlers.Danfe.GetBytesRelatorio(context, Glass.Conversoes.StrParaUint(item.Split(',')[1]), false);
                    
                    if (bytesLib.Count() == 0) 
                        throw new System.Exception("Falha ao recuperar liberações associados ao faturamento");
                    if (bytesNf.Count() == 0)
                        throw new System.Exception("Falha ao recuperar notas fiscais associadas ao faturamento");

                    if (Glass.Data.DAL.NotaFiscalDAO.Instance.GetFormaPagto(null, Glass.Conversoes.StrParaUint(item.Split(',')[1])) == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                    {
                        var bytesBoletos = GetBytesRelatorioBoletos(context, Glass.Conversoes.StrParaUint(item.Split(',')[1]));

                        if(bytesBoletos.Count() == 0)
                            throw new System.Exception("Falha ao recuperar boletos associados ao faturamento");

                        using (var streamLib = new MemoryStream(bytesLib))
                        using (var streamNf = new MemoryStream(bytesNf))
                        using (var streamBoletos = new MemoryStream(bytesBoletos))
                        using (var documentoLib = PdfReader.Open(streamLib, PdfDocumentOpenMode.Import))
                        using (var documentoNf = PdfReader.Open(streamNf, PdfDocumentOpenMode.Import))
                        using (var documentoBoletos = PdfReader.Open(streamBoletos, PdfDocumentOpenMode.Import))
                        {
                            CopyPages(documentoLib, outPdf);
                            CopyPages(documentoNf, outPdf);
                            CopyPages(documentoBoletos, outPdf);
                        }
                    }
                    else
                    {
                        using (var streamLib = new MemoryStream(bytesLib))
                        using (var streamNf = new MemoryStream(bytesNf))
                        using (var documentoLib = PdfReader.Open(streamLib, PdfDocumentOpenMode.Import))
                        using (var documentoNf = PdfReader.Open(streamNf, PdfDocumentOpenMode.Import))
                        {
                            CopyPages(documentoLib, outPdf);
                            CopyPages(documentoNf, outPdf);
                        }
                    }
                }

                outPdf.Save(arquivosConcatenados);
                context.Response.OutputStream.Write(arquivosConcatenados.ToArray(), 0, arquivosConcatenados.ToArray().Length);
            }
        }
    }

    byte[] GetBytesRelatorioBoletos(HttpContext context, uint idNf)
    {
        var codigoNotaFiscal = Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf);
        var codigoContaReceber = 0;

        var idContaBanco = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Financeiro.Negocios.IContaBancariaFluxo>()
                .ObtemContasBanco(0, (int)idNf).FirstOrDefault().Id;

        var codBanco = Glass.Data.DAL.ContaBancoDAO.Instance.ObtemCodigoBanco((uint)idContaBanco);

        var valorPadrao = Glass.Data.DAL.DadosCnabDAO.Instance.ObtemValorPadrao((int)codBanco, (int)idNf, 0);

        var carteira = valorPadrao != null ? valorPadrao.CodCarteira.ToString() :
                Sync.Utils.Boleto.DataSourceHelper.Instance.ObterCarteiras((Sync.Utils.CodigoBanco)codBanco)[0].ToString();

        var especieDoc = valorPadrao != null ? valorPadrao.CodEspecieDocumento.ToString() :
                Sync.Utils.Boleto.DataSourceHelper.Instance.ObterEspecieDocumento(Sync.Utils.Boleto.TipoArquivo.CNAB240,
                (Sync.Utils.CodigoBanco)codBanco)[0].ToString();

        var instrucoes = valorPadrao != null ? valorPadrao.DescInstrucoes : "";

        var urlBoletos =  "../../Handlers/Boleto.ashx?codigoNotaFiscal=" + codigoNotaFiscal +
            "&codigoContaReceber=" + codigoContaReceber + "&codigoContaBanco=" + idContaBanco +
            "&carteira=" + carteira + "&especieDocumento=" + especieDoc + "&instrucoes=" + instrucoes;

        var bytes = Glass.Data.Helper.Utils.GetImageFromRequest(urlBoletos);
        string html = System.Text.Encoding.ASCII.GetString(bytes);
        return new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf(html);
    }


    byte[] GetBytesRelatorioLib(HttpContext context, uint idLiberarPedido)
    {
        var urlLib = "../Relatorios/RelLiberacao.aspx?rel=LiberacaoPedido&semThread=true&EnvioEmail=true&idLiberarPedido=" + idLiberarPedido;
        return Glass.Data.Helper.Utils.GetImageFromRequest(urlLib);
    }

    void CopyPages(PdfDocument from, PdfDocument to)
    {
        for (int i = 0; i < from.PageCount; i++)
        {
            to.AddPage(from.Pages[i]);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}