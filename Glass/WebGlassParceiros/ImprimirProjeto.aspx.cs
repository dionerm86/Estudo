using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class ImprimirProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PedidoDAO.Instance.ObtemIdCliente(Glass.Conversoes.StrParaUint(Request["idPedido"])) != UserInfo.GetUserInfo.IdCliente)
                Response.Redirect("LstPedidos.aspx");
    
            if (!IsPostBack)
                Page.Title += Request["idPedido"];
        }
    }
}
