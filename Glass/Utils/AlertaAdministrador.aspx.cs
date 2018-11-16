using System;

namespace Glass.UI.Web.Utils
{
    public partial class AlertaAdministrador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["pedidosProntosNaoEntregues"] != null && Request["pedidosProntosNaoEntregues"].ToString().ToLower() == "true")
            {
                lblPedido.Visible = true;
                linkPedidos.Visible = true;
            }

            if (Request["boletoVencendo"] != null && Request["boletoVencendo"].ToString().ToLower() == "true")
            {
                lblBoleto.Visible = true;
                linkBoleto.Visible = true;
            }
        }
    }
}
