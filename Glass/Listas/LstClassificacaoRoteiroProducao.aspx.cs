using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstClassificacaoRoteiroProducao : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdClassificacaoRoteiroProducao.Register();
            odsClassificacaoRoteiroProducao.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadClassificacaoRoteiroProducao.aspx");
        }
    }
}