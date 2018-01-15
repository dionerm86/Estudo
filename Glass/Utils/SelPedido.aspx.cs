using System;
using System.Web.UI;

namespace Glass.UI.Web.Utils
{
    public partial class SelPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            if (Request["pcp"] == "1")
                Page.ClientScript.RegisterStartupScript(GetType(), "escondeFiltro", "document.getElementById('filtro').style.display = 'none';\n", true);
        }
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    }
}
