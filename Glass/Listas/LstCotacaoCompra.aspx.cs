using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebGlass.Business.CotacaoCompra.Fluxo;

namespace Glass.UI.Web.Listas
{
    public partial class LstCotacaoCompra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdCotacaoCompra_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Reabrir")
            {
                try
                {
                    uint codigoCotacao = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                    ReabrirCotacaoCompra.Instance.Reabrir(codigoCotacao);
    
                    grdCotacaoCompra.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Cotação de compra reaberta.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao reabrir cotação de compra.", ex, Page);
                }
            }
        }
    }
}
