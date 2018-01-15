using System;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelTextoPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Page.Title += Request["idPedido"];
        }
    
        protected void grdTextoImprPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "IncluiTexto")
            {
                var textoPedido = new TextoPedido
                {
                    IdPedido = Request["idPedido"].StrParaUint(),
                    IdTextoImprPedido = e.CommandArgument.ToString().StrParaUint()
                };
                TextoPedidoDAO.Instance.Insert(textoPedido);
    
                grdTextoPedido.DataBind();
            }
        }

        protected string GetCodigoTabela()
        {
            return ((int)LogCancelamento.TabelaCancelamento.TextoPedido).ToString();
        }
    }
}
