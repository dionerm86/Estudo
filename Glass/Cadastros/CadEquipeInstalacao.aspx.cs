using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadEquipeInstalacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["IdEquipe"] != null)
            {
                dtvEquipe.ChangeMode(DetailsViewMode.Edit);
                hdfIdEquipe.Value = Request["IdEquipe"];
                lnkIncluirIntegrante.Visible = true;
                grdFuncEquipe.Visible = true;
    
                dtvEquipe.ChangeMode(DetailsViewMode.Edit);
                ((Button)dtvEquipe.FindControl("btnAlterarSenha")).Attributes.Add
                    ("OnClick", "openWindow(150, 300, '../Utils/TrocarSenha.aspx?IdEquipe=" + Request["IdEquipe"] + "'); return false;");
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadEquipeInstalacao));
        }
    
        [Ajax.AjaxMethod()]
        public string InsereColocador(string idEquipe, string tipoEquipe, string idFunc, string idTipoFunc)
        {
            return WebGlass.Business.Instalacao.Fluxo.Equipe.Ajax.InsereColocador(idEquipe,
                tipoEquipe, idFunc, idTipoFunc);
        }
    
        protected void odsEquipe_Inserted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar Equipe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("CadEquipeInstalacao.aspx?idEquipe=" + e.ReturnValue.ToString());
        }
    
        protected void odsEquipe_Updated(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao atualizar dados da Equipe.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Response.Redirect("CadEquipeInstalacao.aspx?idEquipe=" + Request["idEquipe"]);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstEquipeInstalacao.aspx");
        }
    
        protected void btnReload_Click(object sender, EventArgs e)
        {
            grdFuncEquipe.DataBind();
        }
    }
}
