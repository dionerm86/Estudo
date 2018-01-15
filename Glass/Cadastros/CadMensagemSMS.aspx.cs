using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMensagemSMS : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    }
}
