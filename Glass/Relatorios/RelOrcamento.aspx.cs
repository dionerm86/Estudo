using System;
using System.Web;
using Glass.Data.Model;
using Glass.Data.DAL;
using Microsoft.Reporting.WebForms;
using Glass.Data.Helper;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelOrcamento : Glass.Relatorios.UI.Web.ReportPage
    {
        protected override object[] Parametros
        {
            get { return new object[] { }; }
        }

        protected override Glass.Relatorios.UI.Web.ReportPage.JavaScriptData DadosJavaScript
        {
            get
            {
                return new JavaScriptData(false, "false");
            }
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            ProcessaReport(pchTabela);
        }
    
        protected override Colosoft.Reports.IReportDocument LoadReport(ref LocalReport report, ref List<ReportParameter> lstParam,
            HttpRequest PageRequest, System.Collections.Specialized.NameValueCollection Request, object[] outrosParametros, LoginUsuario login)
        {
            Orcamento orca = OrcamentoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idOrca"]));
            orca.SomarDescontoProdutosTotal = OrcamentoConfig.ExibirItensProdutosRelatorio;    
            ProdutosOrcamento[] prodOrca = ProdutosOrcamentoDAO.Instance.GetForRpt(Glass.Conversoes.StrParaUint(Request["idOrca"]), true);
    
            uint idLoja = orca.IdLoja != null ? orca.IdLoja.Value : 0;

            // Recupera o caminho do relatório a ser renderizado
            var caminhoRelatorio = string.Format("Relatorios/ModeloOrcamento/rptOrcamento{0}.rdlc", ControleSistema.GetSite().ToString());
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                report.ReportPath = caminhoRelatorio;
            else
                report.ReportPath = PedidoConfig.LiberarPedido ? "Relatorios/ModeloOrcamento/rptOrcamentoLib.rdlc" :
                    "Relatorios/ModeloOrcamento/rptOrcamento.rdlc";

            string rptLogoPath = idLoja > 0 ?
                Logotipo.GetReportLogo(PageRequest, idLoja) : Logotipo.GetReportLogoColorOrca(PageRequest);

            if (Geral.ConsiderarLojaClientePedidoFluxoSistema && orca.IdCliente.GetValueOrDefault(0) > 0)
            {
                var idCliente = orca.IdCliente.Value;

                if (idCliente > 0)
                {
                    var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(idCliente);

                    if (idLojaCliente > 0)
                        rptLogoPath = Logotipo.GetReportLogo(PageRequest, idLojaCliente);
                }
            }
    
            lstParam.Add(new ReportParameter("Logotipo", rptLogoPath));
            lstParam.Add(new ReportParameter("ExibirCusto", "false"));
            lstParam.Add(new ReportParameter("AgruparBeneficiamentos", PedidoConfig.RelatorioPedido.AgruparBenefRelatorio.ToString()));    
            lstParam.Add(new ReportParameter("ExibirIcmsIpi", "True"));
            lstParam.Add(new ReportParameter("Cabecalho_IdOrcamento", orca.IdOrcamento.ToString()));
            lstParam.Add(new ReportParameter("Cabecalho_DataOrcamento", orca.DataOrcamento));
            lstParam.Add(new ReportParameter("Cabecalho_TelefoneLoja", orca.TelefoneLoja));
            lstParam.Add(new ReportParameter("Cabecalho_EmailLoja", orca.EmailLoja));
            lstParam.Add(new ReportParameter("Cabecalho_DadosLoja", orca.DadosLoja));
            lstParam.Add(new ReportParameter("Cabecalho_EnderecoLoja", orca.EnderecoLoja));
            lstParam.Add(new ReportParameter("Cabecalho_FoneFaxLoja", orca.FoneFaxLoja));
            lstParam.Add(new ReportParameter("Cabecalho_SiteLoja", orca.EmailLoja));    
            lstParam.Add(new ReportParameter("FormatTotM", Glass.Configuracoes.Geral.GetFormatTotM()));

            report.DataSources.Add(new ReportDataSource("Orcamento", new Orcamento[] { orca }));
            report.DataSources.Add(new ReportDataSource("ProdutosOrcamento", prodOrca));
            report.DataSources.Add(new ReportDataSource("TextoOrcamento", TextoOrcamentoDAO.Instance.GetByOrcamento(orca.IdOrcamento)));

            return null;
        }
    }
}
