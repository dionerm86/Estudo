using System;
using System.Web;
using Glass.Data.Helper;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.CTe
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
                        Request["rel"] == "DacteLotacao" || Request["rel"] == "DacteFracionada", 
                    "false"
                );
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clientes acessando o sistema externamente não podem entrar neste relatório
            if (UserInfo.GetUserInfo.IdCliente > 0)
                return;
    
            //var cte = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetElement(Glass.Conversoes.StrParaUint(Request["idCte"]));
            //
            //if (cte.TipoEmissao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal)
            //{
            //    if (Request["rel"] == "DacteLotacao")
            //        Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"] + "&isCodigoBarras=true");
            //    else if (Request["rel"] == "DacteFracionada")
            //        Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"] + "&isCodigoBarras=true");
            //}
            //else
            //{
            //    if (Request["rel"] == "DacteLotacao")
            //        Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"]);
            //    else if (Request["rel"] == "DacteFracionada")
            //        Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"]);
            //    else
            //    {
            //        Ajax.Utility.RegisterTypeForAjax(typeof(RelBase));
            //        ProcessaReport(pchTabela);
            //    }
            //}
    
            if (Request["rel"] == "DacteLotacao")
                Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"]);
            else if (Request["rel"] == "DacteFracionada")
                Response.Redirect("~/Handlers/Dacte.ashx?idCte=" + Request["idCte"] + "&previsualizar=" + Request["previsualizar"]);
            else
            {
                ProcessaReport(pchTabela);
            }
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            WebGlass.Business.ConhecimentoTransporte.Entidade.Cte cte = WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarCte.Instance.
                GetCte(Glass.Conversoes.StrParaUint(Request["idCte"]));
            
            // Verifica qual relatório será chamado
            switch (Request["rel"])
            {
                case "CteTerceiros":
                    report.ReportPath = "Relatorios/CTe/rptCTeTerceiros.rdlc";
                    report.DataSources.Add(new ReportDataSource("Cte", new[] { cte }));
                    report.DataSources.Add(new ReportDataSource("CobrancaDuplCte", cte.ObjCobrancaCte.ObjCobrancaDuplCte));
                    report.DataSources.Add(new ReportDataSource("CobrancaCte", new[] { cte.ObjCobrancaCte }));
                    report.DataSources.Add(new ReportDataSource("SeguroCte", new[] { cte.ObjSeguroCte }));
                    report.DataSources.Add(new ReportDataSource("EntregaCte", new[] { cte.ObjEntregaCte }));
                    report.DataSources.Add(new ReportDataSource("InfoCte", new[] { cte.ObjInfoCte }));
                    report.DataSources.Add(new ReportDataSource("InfoCargaCte", cte.ObjInfoCte.ObjInfoCargaCte));
                    report.DataSources.Add(new ReportDataSource("ImpostoCte", cte.ObjImpostoCte));
                    report.DataSources.Add(new ReportDataSource("ConhecimentoTransporteRodoviario", new[] { cte.ObjConhecimentoTransporteRodoviario }));
                    report.DataSources.Add(new ReportDataSource("ComplCte", new[] { cte.ObjComplCte }));
                    report.DataSources.Add(new ReportDataSource("ParticipanteCte", cte.ObjParticipanteCte));
                    report.DataSources.Add(new ReportDataSource("EfdCte", new[] { cte.ObjEfdCte }));
                    report.DataSources.Add(new ReportDataSource("NfeCte", WebGlass.Business.ConhecimentoTransporte.Fluxo.BuscarNotaFiscalCte.Instance.GetForRpt(cte.IdCte)));
                    break;
            }

            // Atribui parâmetros ao relatório
            lstParam.Add(new ReportParameter("Logotipo",
                FiscalConfig.ConhecimentoTransporte.ExibirLogomarcaNoDacte ?
                    Logotipo.GetReportLogo(PageRequest) :
                    ""));
            lstParam.Add(new ReportParameter("TextoRodape", Geral.TextoRodapeRelatorio(login.Nome)));
            lstParam.Add(new ReportParameter("CorRodape", "DimGray"));

            return null;
        }
    }
}
