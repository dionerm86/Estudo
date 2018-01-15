using System;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelEtiquetaVolume : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { txtObs.Text }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    Request.QueryString["init"] == "1" && !bool.Parse(hdfInit.Value),
                    "document.getElementById('" + hdfLoad.ClientID + "').value == 'false'"
                );
            }
        }
    
        private bool _loadSubreport = true;
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DadosJavaScript.BackgroundLoading)
                return;
    
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
           HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "EtqVolume":
                    var caminhoRelatorio = string.Format("Relatorios/ModeloEtiquetaVolume/rptEtiquetaVolume{0}.rdlc", ControleSistema.GetSite().ToString());

                    if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                        report.ReportPath = caminhoRelatorio;
                    else
                        report.ReportPath = "Relatorios/ModeloEtiquetaVolume/rptEtiquetaVolume.rdlc";

                    var etqVolume = WebGlass.Business.OrdemCarga.Fluxo.EtiquetaVolumeFluxo.Instance.GetForImpressao(Glass.Conversoes.StrParaUint(Request["IdVolume"]));
                    report.DataSources.Add(new ReportDataSource("EtiquetaVolume", etqVolume));
                    break;
    
                default:
                    break;
            }

            return null;
        }
    
        protected override void report_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            // Só carrega o subreport do relatório de Produtos Comprados se o checkbox de exibir detalhes estiver marcado
            if (!_loadSubreport)
                e.DataSources.Add(new ReportDataSource("Produto", new Produto[0]));
            else
                base.report_SubreportProcessing(sender, e);
        }
    }
}
