using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstHistoricoFornec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    
        protected void grdConta_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdConta.Rows)
            {
                string color = ((HiddenField)row.Cells[0].FindControl("hdfColor")).Value;
    
                if (color == "Red")
                    foreach (TableCell c in row.Cells) c.ForeColor = System.Drawing.Color.Red;
                else if (color == "Green")
                    foreach (TableCell c in row.Cells) c.ForeColor = System.Drawing.Color.Green;
                else
                    foreach (TableCell c in row.Cells) c.ForeColor = System.Drawing.Color.Blue;
            }
    
            if (grdConta.Rows.Count > 0)
            {
                tbTotais.Visible = true;
                lnkImprimir.Visible = true;
    
                lblTotalAberto.Text = ((HiddenField)grdConta.Rows[0].FindControl("hdfTotalEmAberto")).Value.ToString();
                lblTotalRecEmDia.Text = ((HiddenField)grdConta.Rows[0].FindControl("hdfTotalRecEmDia")).Value.ToString();
                lblTotalRecComAtraso.Text = ((HiddenField)grdConta.Rows[0].FindControl("hdfTotalRecComAtraso")).Value.ToString();
            }
            else
            {
                tbTotais.Visible = false;
                lnkImprimir.Visible = false;
            }
        }
    
        protected void btnLimparFiltros_Click(object sender, EventArgs e)
        {
            txtNumFornec.Text = "";
            txtNomeFornec.Text = "";
    
            ((TextBox)ctrlDataIniVenc.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataFimVenc.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataIniPag.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataFimPag.FindControl("txtData")).Text = String.Empty;
            txtPrecoInicialVenc.Text = String.Empty;
            txtPrecoFinalVenc.Text = String.Empty;
            txtPrecoInicialPag.Text = String.Empty;
            txtPrecoFinalPag.Text = String.Empty;
    
            chkEmAberto.Checked = true;
            chkPagasEmDia.Checked = true;
            chkPagasComAtraso.Checked = true;
    
            grdConta.DataBind();
        }
    }
}
