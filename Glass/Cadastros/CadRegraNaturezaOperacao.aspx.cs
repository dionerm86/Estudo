using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadRegraNaturezaOperacao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsRegraNaturezaOperacao.Register();
            dtvRegraNaturezaOperacao.Register("~/Listas/LstRegraNaturezaOperacao.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request["id"] != null)
                dtvRegraNaturezaOperacao.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Listas/LstRegraNaturezaOperacao.aspx");
        }
    
        protected void ctrlSelCorProd_Load(object sender, EventArgs e)
        {
            var grupoSubgrupo = dtvRegraNaturezaOperacao.FindControl("ctrlSelGrupoSubgrupoProd");
            if (grupoSubgrupo == null)
                return;
    
            var grupo = grupoSubgrupo.FindControl("drpGrupoProd");
            
            var ctrl = sender as Glass.UI.Web.Controls.ctrlSelCorProd;
            ctrl.ControleGrupo = grupo;
        }
    }
}
