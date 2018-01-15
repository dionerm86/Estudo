using System;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaEstoqueFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
        }
    
        protected void imgPesq_Click(object sender, EventArgs e)
        {
            grdEstoque.PageIndex = 0;
        }
    
        protected void drpGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdEstoque.PageIndex = 0;
        }
    }
}
