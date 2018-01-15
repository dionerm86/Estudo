using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class MovimentacoesFinanceiras : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
    
            dataFim.Visible = chkDetalhado.Checked;
            lblData.Text = chkDetalhado.Checked ? "Período" : "Data";
            grdMovFinanc.Visible = !chkDetalhado.Checked;
            grdMovFinancDet.Visible = chkDetalhado.Checked;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdMovFinanc.PageIndex = 0;
            grdMovFinancDet.PageIndex = 0;
        }
    }
}
