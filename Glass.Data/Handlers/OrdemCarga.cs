using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.Handlers
{
    public class OrdemCarga
    {
        public static byte[] GetBytesRelatorio(HttpContext context, uint idOC)
        {
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            return GetBytesRelatorio(context, idOC, out warnings, out streamids, out mimeType, out encoding, out extension);
        }

        public static byte[] GetBytesRelatorio(HttpContext context, uint idOC, out Warning[] warnings, out string[] streamids,
            out string mimeType, out string encoding, out string extension)
        {
            LocalReport report = new LocalReport();
            List<ReportParameter> lstParam = new List<ReportParameter>();

            report.ReportPath = "Relatorios/OrdemCarga/rptOrdemCarga.rdlc";
            report.ReportPath = context.Server.MapPath("~/" + report.ReportPath);

            report.SubreportProcessing += new SubreportProcessingEventHandler(report_SubreportProcessing);

            lstParam.Add(new ReportParameter("ExibirEnderecoCliente", OrdemCargaConfig.ExibirEnderecoClienteRptOC.ToString()));

            var oc = OrdemCargaDAO.Instance.GetForRptInd(idOC);
            var pedidosOC = oc[0].Pedidos.ToArray();
            var produtosOC = ProdutosPedidoDAO.Instance.GetByPedidosForOcRpt(oc[0].IdOrdemCarga, oc[0].IdsPedidos);
            report.DataSources.Add(new ReportDataSource("OrdemCarga", oc));
            report.DataSources.Add(new ReportDataSource("Pedido", pedidosOC));
            report.DataSources.Add(new ReportDataSource("ProdutosPedido", produtosOC));

            // Atribui parâmetros ao relatório
            report.EnableExternalImages = true;

            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(context.Request)));
            lstParam.Add(new ReportParameter("TextoRodape", "WebGlass v" + Geral.ObtemVersao() + " - Relatório gerado automaticamente  em " +
                DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            report.SetParameters(lstParam);

            return report.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        }

        private static void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // Recupera o relatório pai
            LocalReport report = (LocalReport)sender;

            // Carrega os DataSources do pai
            foreach (string dataSource in e.DataSourceNames)
                e.DataSources.Add(report.DataSources[dataSource]);
        }
    }
}
