using Glass.Data.Model;
using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstPerdaChapaVidro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void odsPerdaChapaVidro_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg(null, e.Exception, Page);
                e.ExceptionHandled = true;
            }

            grdPerdaChapaVidro.DataBind();
        }

        protected void grdPerdaChapaVidro_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            var perda = e.Row.DataItem as PerdaChapaVidro;

            if (perda.Cancelado)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadPerdaChapaVidro.aspx");
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPerdaChapaVidro.PageIndex = 0;
            grdPerdaChapaVidro.DataBind();
        }
    }
}