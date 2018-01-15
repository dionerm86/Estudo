using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstDevolucaoPagto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadDevolucaoPagto.aspx" + (Request["caixaDiario"] == "true" ? "?caixaDiario=true" : ""));
        }
    }
}
