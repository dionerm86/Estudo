using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class ShowLogAlteracao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    int tabela = Glass.Conversoes.StrParaInt(Request["tabela"]);
                    string descrTabela = LogAlteracao.GetDescrTabela(tabela);
                    Page.Title += descrTabela;
                }
                catch { }
            }
    
            hdfExibirAdmin.Value = UserInfo.GetUserInfo.IsAdminSync.ToString();
        }
    
        protected void odsCampos_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            drpCampo.SelectedIndex = 0;
            KeyValuePair<string, string>[] dados = (KeyValuePair<string, string>[])e.ReturnValue;
            filtroCampo.Visible = dados.Length > 1;

            if (!string.IsNullOrEmpty(Request["campo"]) && dados.Length == 1)
                drpCampo.SelectedIndex = 1;
        }
    
        protected void grdLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (String.IsNullOrEmpty(lblSubtitle.Text) && e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = ((LogAlteracao)e.Row.DataItem).Referencia;
                if (!String.IsNullOrEmpty(item))
                    lblSubtitle.Text = item.Replace(" ", "&nbsp;");
            }
        }
    }
}
