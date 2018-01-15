using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadPedidoEspelhoGerar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnGerarEspelho_Click(object sender, EventArgs e)
        {
            uint idPedido = Glass.Conversoes.StrParaUint(txtNumPedido.Text);

            try
            {    
                PedidoEspelhoDAO.Instance.GeraEspelhoComTransacao(idPedido);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao gerar Conferência do Pedido.", ex, Page);
                return;
            }

            Response.Redirect("CadPedidoEspelho.aspx?idPedido=" + idPedido, false);
        }
    }
}
