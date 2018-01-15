using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelMedicao : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    Request.QueryString["init"] == "1" && !bool.Parse(hdfInit.Value),
                    "document.getElementById('" + hdfLoad.ClientID + "').value == 'true'"
                );
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (DadosJavaScript.BackgroundLoading)
                return;
    
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            Medicao medicao = MedicaoDAO.Instance.GetForRptUnico(Glass.Conversoes.StrParaUint(Request["idMedicao"]));

            // Define qual relatório será exibido
            report.ReportPath = Glass.Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloMedicao/rptMedicao{0}.rdlc");
    
            // Atribui medição ao relatório
            report.DataSources.Add(new ReportDataSource("Medicao", new Medicao[] { medicao }));
    
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));

            return null;
        }
    }
}
