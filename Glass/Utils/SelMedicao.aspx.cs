using System;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class SelMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    }
}
