using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFinalizarConferencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPedidosConferencia.PageIndex = 0;
        }
    
        protected void drpLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdPedidosConferencia.PageIndex = 0;
        }
    
        protected void grdPedidosConferencia_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Finalizar")
            {
                try
                {
                    PedidoConferenciaDAO.Instance.FinalizarConferencia(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
                    grdPedidosConferencia.DataBind();
                    Glass.MensagemAlerta.ShowMsg("Conferência Finalizada.", Page);
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao finalizar conferência.", ex, Page);
                }
            }
        }
    }
}
