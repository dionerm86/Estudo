using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using System.Drawing;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstEtiquetaIndImp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProduto.PageIndex = 0;
        }
    
        protected void grdProduto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            ProdutosPedidoEspelho ppe = e.Row.DataItem as ProdutosPedidoEspelho;
            if (ppe == null)
                return;
    
            if (PedidoDAO.Instance.IsPedidoReposicao(null, ppe.IdPedido.ToString()))
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }
    
        public string PodeImprimir()
        {
            return (Config.PossuiPermissao(Config.FuncaoMenuPCP.ReimprimirEtiquetas) ||
                UserInfo.GetUserInfo.IsAdministrador).ToString().ToLower();
        }
    }
}
