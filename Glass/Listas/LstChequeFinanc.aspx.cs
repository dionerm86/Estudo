using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstChequeFinanc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadChequeFinanc.aspx?" + "caixaDiario=" + Request["caixaDiario"]);
        }
    }
}
