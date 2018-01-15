using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetCFOPProdutoGerarNf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Utils.SetCFOPProdutoGerarNf));
        }
    
        protected void odsCfop_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).SelectedValue = Request["idcfop"];
        }
    }
}
