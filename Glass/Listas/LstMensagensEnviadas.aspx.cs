using System;

namespace Glass.UI.Web.Listas
{
    public partial class LstMensagensEnviadas : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdMensagem.Register();
            odsMensagem.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    }
}
