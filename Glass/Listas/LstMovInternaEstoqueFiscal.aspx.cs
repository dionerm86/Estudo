using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstMovInternaEstoqueFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadMovInternaEstoqueFiscal.aspx");
        }
    }
}