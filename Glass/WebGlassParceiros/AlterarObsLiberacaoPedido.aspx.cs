using System;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class AlterarObsLiberacaoPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                txtObs.Text = Data.DAL.PedidoDAO.Instance.ObtemObsLiberacao(Conversoes.StrParaUint(Request["idPedido"]));
        }   
      
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            Data.DAL.PedidoDAO.Instance.SalvarObsLiberacao(Conversoes.StrParaUint(Request["idPedido"]), txtObs.Text);
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "alert('Observação salva com sucesso.'); closeWindow();", true);

        }
    }
}
