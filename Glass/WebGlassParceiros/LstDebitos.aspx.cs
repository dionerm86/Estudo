using System;
using System.Web.UI;
using Glass.Configuracoes;
using System.Web.UI.WebControls;

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

        protected void lblCreditoCliente_Load(object sender, EventArgs e)
        {
            var idCliente = Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente;

            ((Label)sender).Text = string.Format("Você possui R$ {0} de crédito com esse fornecedor", MetodosAjax.GetClienteCredito(idCliente.ToString()));
        }
    }
}
