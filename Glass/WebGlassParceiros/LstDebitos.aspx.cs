using System;
using System.Web.UI;
using Glass.Configuracoes;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class LstDebitos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!PedidoConfig.LiberarPedido)
            {
                grdConta.Columns[1].Visible = false;
                Label2.Visible = false;
                txtIdLiberarPedido.Visible = false;
                imgPesq2.Visible = false;
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
        }
    }
}
