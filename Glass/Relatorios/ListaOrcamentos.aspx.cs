using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaOrcamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (drpSituacao.Text == "0")
            {
                lblPeriodoSituacao.Visible = false;
                ((TextBox)ctrlDataIniSit.FindControl("txtData")).Text = "";
                ctrlDataIniSit.Visible = false;
                ((TextBox)ctrlDataFimSit.FindControl("txtData")).Text = "";
                ctrlDataFimSit.Visible = false;
                imgPesqPeriodoSituacao.Visible = false;
            }
            else
            {
                lblPeriodoSituacao.Visible = true;
                ctrlDataIniSit.Visible = true;
                ctrlDataFimSit.Visible = true;
                imgPesqPeriodoSituacao.Visible = true;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }

        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdOrcamento.PageIndex = 0;
        }
    }
}
