using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaContasPagas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                lnkExportarGCON.Visible = FinanceiroConfig.GerarArquivoGCon;
                lnkExportarProsoft.Visible = FinanceiroConfig.GerarArquivoProsoft;
                lnkExportarDominio.Visible = FinanceiroConfig.GerarArquivoDominio;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void grdConta_Sorted(object sender, EventArgs e)
        {
            hdfOrdenar.Value = grdConta.SortExpression + (grdConta.SortDirection == SortDirection.Descending ? " Desc" : "");
        }
    
        protected void grdConta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            if (!((ContasPagar)e.Row.DataItem).Paga)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Blue;
        }
    
        protected void drpTipo_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, "1"));
                drpTipo.Items.Add(new ListItem(FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil, "2"));
            }
        }
    }
}
