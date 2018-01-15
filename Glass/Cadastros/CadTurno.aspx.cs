using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadTurno : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvTurno.Register("~/Listas/LstTurno.aspx");
            odsTurno.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request["idTurno"] != null)
                dtvTurno.ChangeMode(DetailsViewMode.Edit);
        }
    
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstTurno.aspx");
        }
    }
}
