using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Utils_AlertaAdministrador : System.Web.UI.Page
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