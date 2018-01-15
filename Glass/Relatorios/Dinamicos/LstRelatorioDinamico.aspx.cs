using System;

namespace Glass.UI.Web.Relatorios.Dinamicos
{
    public partial class LstRelatorioDinamico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadRelatorioDinamico.aspx");
        }
    }
}