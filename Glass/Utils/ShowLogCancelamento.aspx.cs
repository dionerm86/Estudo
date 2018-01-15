using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class ShowLogCancelamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LogCancelamento.TabelaCancelamento tabela = (LogCancelamento.TabelaCancelamento)Glass.Conversoes.StrParaInt(Request["tabela"]);
    
            if (!IsPostBack)
            {
                try
                {
                    string descrTabela = LogCancelamento.GetDescrTabela(tabela);
                    Page.Title += descrTabela;
                }
                catch { }
            }
    
            if (!IsPostBack)
                hdfIdRegistroCanc.Value = Request.QueryString["id"];
            
            hdfExibirAdmin.Value = UserInfo.GetUserInfo.IsAdminSync.ToString();
    
            switch (tabela)
            {
                case LogCancelamento.TabelaCancelamento.MovimentacaoBancaria:
                    grdLog.Columns[0].Visible = true;
                    grdLog.Columns[0].HeaderText = "Cód.";
                    break;
                default:
                    grdLog.Columns[0].Visible = false;
                    break;
            }
    
            lblIdRegistroCanc.Visible = grdLog.Columns[0].Visible;
            lblIdRegistroCanc.Text = grdLog.Columns[0].HeaderText;
            txtIdRegistroCanc.Visible = grdLog.Columns[0].Visible;
            imgPesq.Visible = grdLog.Columns[0].Visible;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (String.IsNullOrEmpty(lblSubtitle.Text) && e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = ((LogCancelamento)e.Row.DataItem).Referencia;
                if (!String.IsNullOrEmpty(item))
                    lblSubtitle.Text = item.Replace(" ", "&nbsp;");
            }
        }
    }
}
