using System;

namespace Glass.UI.Web.Otimizacao.Controls
{
    public partial class PecasOtimizadas : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["pedidos"] != null)
            {
                string[] pedidos = Request["pedidos"].ToString().Split(',');
            }
        }
    }
}
