using System;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaContasRecebidas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["rel"] == "1")
                grdConta.Columns[0].Visible = false;
        }
    }
}
