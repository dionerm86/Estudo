using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaFornecedores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdClientes.PageIndex = 0;
        }
    }
}
