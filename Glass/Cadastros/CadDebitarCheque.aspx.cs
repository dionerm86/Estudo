using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadDebitarCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void drpContaBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected void odsCheques_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                grdCheque.DataBind();
                Glass.MensagemAlerta.ErrorMsg("Falha ao quitar cheque.", e.Exception, Page);
            }
            else
            {
                grdCheque.DataBind();
                Glass.MensagemAlerta.ShowMsg("Cheque Quitado.", Page);
            }
        }
    }
}
