using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstRetificarSinalCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfIdSinalCompra.Value = selSinal.Valor;
        }
    
        protected void grdCompras_DataBound(object sender, EventArgs e)
        {
            caption.Visible = grdCompras.Rows.Count > 0;
            btnRetificarSinal.Visible = grdCompras.Rows.Count > 0;
        }
    
        protected void btnRetificarSinal_Click(object sender, EventArgs e)
        {
            try
            {
                uint idSinalCompra = Glass.Conversoes.StrParaUint(hdfIdSinalCompra.Value);
                SinalCompraDAO.Instance.RetificaSinal(idSinalCompra, hdfIdsCompras.Value);
    
                Response.Redirect(Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao retificar sinal.", ex, Page);
            }
        }
    }
}
