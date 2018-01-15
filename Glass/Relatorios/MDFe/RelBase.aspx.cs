using System;
using System.Web;
using Glass.Data.Helper;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.MDFe
{
    public partial class RelBase : Glass.Relatorios.UI.Web.ReportPage
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
                    UserInfo.GetUserInfo.IdCliente > 0 || 
                        Request["rel"] == "Damdfe", 
                    "false"
                );
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clientes acessando o sistema externamente não podem entrar neste relatório
            if (UserInfo.GetUserInfo.IdCliente > 0)
                return;
    
            if (Request["rel"] == "Damdfe")
                Response.Redirect("~/Handlers/Damdfe.ashx?IdMDFe=" + Request["IdMDFe"] + "&previsualizar=" + Request["previsualizar"]);
            else
            {
                ProcessaReport(pchTabela);
            }
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos)
        {
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}
