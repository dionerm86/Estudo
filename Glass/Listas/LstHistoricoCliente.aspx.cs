using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Glass.UI.Web.Listas
{
    public partial class LstHistoricoCliente : System.Web.UI.Page
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
                Color color;
                switch (((HiddenField)row.Cells[0].FindControl("hdfColor")).Value)
                {
                    case "Red":
                        color = Color.Red;
                        break;
    
                    case "Green":
                        color = Color.Green;
                        break;
    
                    default:
                        color = Color.Blue;
                        break;
                }
    
                foreach (TableCell c in row.Cells)
                    c.ForeColor = color;
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
            txtNumCli.Text = "";
            txtNomeCliente.Text = "";
            
            ((TextBox)ctrlDataIniVenc.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataFimVenc.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataIniRec.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataFimRec.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataIniCad.FindControl("txtData")).Text = String.Empty;
            ((TextBox)ctrlDataFimCad.FindControl("txtData")).Text = String.Empty;
            txtPrecoInicialVenc.Text = String.Empty;
            txtPrecoFinalVenc.Text = String.Empty;
            txtPrecoInicialRec.Text = String.Empty;
            txtPrecoFinalRec.Text = String.Empty;
    
            chkEmAberto.Checked = true;
            chkRecebidasEmDia.Checked = true;
            chkRecebidasComAtraso.Checked = true;
    
            grdConta.DataBind();
        }
    }
}
