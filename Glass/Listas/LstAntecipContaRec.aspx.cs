using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstAntecipContaRec : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void grdAntecipContaRec_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "Cancelar")
            //{
            //    try
            //    {
            //        // Cancela a antecipação
            //        AntecipContaRecDAO.Instance.Cancelar(Glass.Conversoes.StrParaUint(e.CommandArgument.ToString()));
    
            //        Glass.MensagemAlerta.ShowMsg("Antecipação cancelada.", Page);
    
            //        grdAntecipContaRec.DataBind();
            //    }
            //    catch (Exception ex)
            //    {
            //        Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar Antecipação.", ex, Page);
            //    }
            //}
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadAntecipContaRec.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdAntecipContaRec.PageIndex = 0;
        }
    }
}
