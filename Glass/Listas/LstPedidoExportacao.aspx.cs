using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidoExportacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void lnkPesquisar_Click(object sender, ImageClickEventArgs e)
        {
            grdPedido.PageIndex = 0;
        }
    
        protected void grdPedido_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Chegou")
            {
                uint idPedido = Convert.ToUInt32(e.CommandArgument);
    
                PedidoExportacaoDAO.Instance.InserirSituacaoExportado(idPedido, (int)PedidoExportacao.SituacaoExportacaoEnum.Chegou);
    
                grdPedido.DataBind();
            }
        }
    }
}
