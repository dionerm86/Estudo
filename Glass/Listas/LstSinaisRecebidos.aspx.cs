using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstSinaisRecebidos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request["antecipado"] == "1")
            {
                this.Page.Title = "Pagamentos Antecipados de Pedido Recebidos";
            }
        }
    }
}
