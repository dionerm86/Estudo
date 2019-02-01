using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstAcertoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["pagto"] == "1")
                Page.Title = "Acerto de Cheques Próprios Devolvidos/Abertos";
        }
    }
}
