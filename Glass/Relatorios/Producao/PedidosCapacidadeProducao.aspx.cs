using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class PedidosCapacidadeProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ctrlData.Data = DateTime.Today;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
