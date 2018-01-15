using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class Perdas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPerda.PageIndex = 0;
        }
    }
}
