using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadContaBanco : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvContaBanco.Register("~/Listas/LstContaBanco.aspx");
            odsContaBanco.Register();

            if (Request["idContaBanco"] != null)
                dtvContaBanco.ChangeMode(DetailsViewMode.Edit);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstContaBanco.aspx");
        }
    }
}
