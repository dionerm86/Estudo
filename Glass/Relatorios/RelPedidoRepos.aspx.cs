using System;
using System.Web;
using Microsoft.Reporting.WebForms;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.RelDAL;
using Glass.Data.RelModel;
using System.Collections.Generic;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class RelPedidoRepos : Glass.Relatorios.UI.Web.ReportPage
    {
        public enum TipoRelatorioPedido
        {
            Normal,
            MemoriaCalculo,
            Pcp
        }
    
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
            uint idPedido = Glass.Conversoes.StrParaUint(Request["idPedido"]);
            TipoRelatorioPedido tipo = !String.IsNullOrEmpty(Request["tipo"]) ? (TipoRelatorioPedido)Glass.Conversoes.StrParaInt(Request["tipo"]) : TipoRelatorioPedido.Normal;
            Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetForRpt(idPedido.ToString(), tipo == TipoRelatorioPedido.Pcp)[0];
            ProdutosPedido[] prodPedido = ProdutosPedidoDAO.Instance.GetForRpt(idPedido, tipo == TipoRelatorioPedido.Pcp, false);
            var parcPedido = ParcelasPedidoDAO.Instance.GetForRpt(idPedido);
            PedidoReposicao pedidoRepos = PedidoReposicaoDAO.Instance.GetByPedido(idPedido);

            report.ReportPath = Data.Helper.Utils.CaminhoRelatorio("Relatorios/ModeloPedidoReposicao/rptPedidoReposicao{0}.rdlc");
    
            lstParam.Add(new ReportParameter("Logotipo", Logotipo.GetReportLogo(PageRequest, pedido.IdLoja)));
            lstParam.Add(new ReportParameter("TipoRpt", ((int)tipo).ToString()));
    
            report.DataSources.Add(new ReportDataSource("Pedido", new Glass.Data.Model.Pedido[] { pedido }));
            report.DataSources.Add(new ReportDataSource("PedidoReposicao", new PedidoReposicao[] { pedidoRepos }));
            report.DataSources.Add(new ReportDataSource("ParcelasPedido", parcPedido));
            report.DataSources.Add(new ReportDataSource("ProdutosReposicao", ProdutosReposicaoDAO.GetByProdutosPedido(prodPedido)));
    
            report.DataSources.Add(new ReportDataSource("TipoPerda", TipoPerdaDAO.Instance.GetByPedidoRepos(idPedido)));
            report.DataSources.Add(new ReportDataSource("TipoPerdaRepos", new TipoPerdaRepos[] { TipoPerdaReposDAO.GetByPedidoRepos(idPedido) }));

            return null;
        }
    }
}
