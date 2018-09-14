using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstEstoque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["fiscal"] == "1")
                Page.Title = "Estoque Fiscal de Produtos";
        }
    }
}
