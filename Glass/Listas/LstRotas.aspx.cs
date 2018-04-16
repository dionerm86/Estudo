using System;
using Glass.Data.DAL;

namespace Glass.UI.Web.Listas
{
    public partial class LstRotas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Listas.LstRotas));
    
            txtDataFim.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
