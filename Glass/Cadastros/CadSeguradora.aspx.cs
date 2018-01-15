using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSeguradora : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvCadSeguradora.Register("~/Listas/LstSeguradora.aspx");
            odsSeguradora.Register();

            if (Request["idSeguradora"] != null)
                dtvCadSeguradora.ChangeMode(DetailsViewMode.Edit);

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstSeguradora.aspx");
        }
    }
}
