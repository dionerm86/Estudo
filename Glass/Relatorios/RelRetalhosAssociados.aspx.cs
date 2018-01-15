using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using Glass.Data.RelModel;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelRetalhosAssociados : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return hdfTabelaProdutos.Value.TrimEnd('|').Split('|'); }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(String.IsNullOrEmpty(hdfTabelaProdutos.Value),
                    "false"
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
            List<RetalhoAssociado> lstRetalho = new List<RetalhoAssociado>();
            RetalhoAssociado retalho;
    
            foreach (string item in outrosParametros)
            {
                string[] campos = item.Split('\t');
    
                retalho = new RetalhoAssociado()
                {
                    IdPedido = Glass.Conversoes.StrParaUint(campos[0]),
                    DescricaoProduto = campos[1],
                    LarguraAltura = campos[2],
                    TotM = Glass.Conversoes.StrParaFloat(campos[3]),
                    Proc = campos[4],
                    Apl = campos[5],
                    Qtde = Glass.Conversoes.StrParaInt(campos[6]),
                    NumeroPeca = Glass.Conversoes.StrParaInt(campos[7]),
                    EtiquetaRetalhoSelecionado = campos.Length > 9 ? campos[8] : null,
                    RetalhoSelecionado = campos.Length > 9 ? campos[9] : campos[8]
                };
    
                lstRetalho.Add(retalho);
            }

            report.ReportPath = "Relatorios/rptRetalhosAssociados.rdlc";
            report.DataSources.Add(new ReportDataSource("RetalhoAssociado", lstRetalho));
    
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}
