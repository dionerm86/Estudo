using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstDepositoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdDepositoNaoIdentificado.DataBind();
        }
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadDepositoNaoIdentificado.aspx");
        }
    }
}
