using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstArquivoRemessa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void odsArquivoRemessa_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }
        }
        protected void grdArquivoRemessa_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var corLinha = ((Glass.Data.Model.ArquivoRemessa)e.Row.DataItem).CorLinha;
    
                foreach (TableCell cell in e.Row.Cells)
                    cell.ForeColor = corLinha;
            }
        }

        protected void lnkPesquisar_Click(object sender, EventArgs e)
        {
            grdArquivoRemessa.PageIndex = 0;
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdArquivoRemessa.PageIndex = 0;
        }
    }
}
