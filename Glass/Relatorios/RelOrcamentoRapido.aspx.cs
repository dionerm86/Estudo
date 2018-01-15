using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelOrcamentoRapido : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return hdfTabelaProdutos.Value.TrimEnd('|').Split('|'); }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(
                    String.IsNullOrEmpty(hdfTabelaProdutos.Value),
                    "document.getElementById('" + hdfTabelaProdutos.ClientID + "').value == ''"
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
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login, string diretorioLogotipos = "")
        {
            List<OrcamentoRapido> lstOrca = new List<OrcamentoRapido>();
            OrcamentoRapido orca;
    
            foreach (string item in outrosParametros)
            {
                string[] campos = item.Split('\t');
    
                orca = new OrcamentoRapido();
                orca.Descricao = campos[0];
                orca.Qtde = Glass.Conversoes.StrParaInt(campos[1]);
                orca.Servicos = campos[5];
                orca.ValorUnitario = Glass.Conversoes.StrParaDecimal(campos[6]);
                orca.Total = Glass.Conversoes.StrParaDecimal(campos[7]);
    
                if (!String.IsNullOrEmpty(campos[2]))
                    orca.Altura = float.Parse(campos[2], System.Globalization.NumberStyles.Any);
    
                if (!String.IsNullOrEmpty(campos[3]))
                    orca.Largura = Glass.Conversoes.StrParaInt(campos[3]);
    
                if (!String.IsNullOrEmpty(campos[4]))
                {
                    string totM2 = campos[4];
                    string totM2Calc = totM2.IndexOf(" (") > -1 ? totM2.Substring(totM2.IndexOf(" (") + 2) : "0";
    
                    if (totM2.IndexOf(")") > -1)
                    {
                        totM2 = totM2.Substring(0, totM2.IndexOf(" ("));
                        totM2Calc = totM2Calc.Substring(0, totM2Calc.Length - 1);
                    }

                    orca.TotM2 = Glass.Conversoes.StrParaDouble(totM2);
                    orca.TotM2Calc = Glass.Conversoes.StrParaDouble(totM2Calc);
                }
    
                lstOrca.Add(orca);
            }
            
            report.ReportPath = "Relatorios/rptOrcamentoRapido.rdlc";
            report.DataSources.Add(new ReportDataSource("OrcamentoRapido", lstOrca));
    
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest)));    
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}