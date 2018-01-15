using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Microsoft.Reporting.WebForms;
using Glass.Configuracoes;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class RelBase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LocalReport report = new LocalReport();
    
            List<ReportParameter> lstParam = new List<ReportParameter>();
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo()));
            report.EnableExternalImages = true;
    
            uint? idCliente = UserInfo.GetUserInfo.IdCliente;
    
            // Se não for um cliente que estiver logado, não imprime relatório
            if (idCliente == null || idCliente == 0)
                return;
    
            if (Request["rel"] == "Danfe")
            {
                Response.Redirect("~/Handlers/Danfe.ashx?idNf=" + Request["idNf"] + "&previsualizar=" + Request["previsualizar"]);
                return;
            }
    
            uint idFunc = UserInfo.GetUserInfo.CodUser;
    
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "Debitos":
                    report.ReportPath = "Relatorios/rptDebitosCliente.rdlc";
                    var debitos = ContasReceberDAO.Instance.GetDebitosList(idCliente.Value, 
                        Glass.Conversoes.StrParaUint(Request["idPedido"]), Glass.Conversoes.StrParaUint(Request["idLiberarPedido"]), null, 0, 0, null, 
                        null, null, 0, 100000);
                    report.DataSources.Add(new ReportDataSource("ContasReceber", debitos));
                    lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(FuncionarioDAO.Instance.GetNome(idFunc))));
                    lstParam.Add(new ReportParameter("CorRodape", "DimGray"));
                    lstParam.Add(new ReportParameter("LiberarPedido", PedidoConfig.LiberarPedido.ToString()));
                    break;
                default:
                    throw new Exception("Nenhum relatório especificado.");
            }
    
            report.SetParameters(lstParam);
    
            Warning[] Warnings = null;
            string[] StreamIds = null;
            string MimeType = null;
            string Encoding = null;
            string Extension = null;
            byte[] bytes = null;
    
            bytes = report.Render("PDF", null, out MimeType, out Encoding, out Extension, out StreamIds, out Warnings);
    
            try
            {
                // Utilizado para exibir na tela
                Response.Clear();
                Response.ContentType = MimeType;
                Response.AddHeader("Content-disposition", "inline");
                Response.BinaryWrite(bytes);
                Response.End();
            }
            catch (Exception ex)
            {
                ex.Data["warnings"] = Warnings;
                ex.Data["streamids"] = StreamIds;
                ex.Data["mimetype"] = MimeType;
                ex.Data["encoding"] = Encoding;
                ex.Data["extension"] = Extension;
                throw ex;
            }
        }
    }
}
