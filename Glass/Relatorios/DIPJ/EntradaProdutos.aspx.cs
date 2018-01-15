using System;

namespace Glass.UI.Web.Relatorios.DIPJ
{
    public partial class EntradaProdutos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataEmissaoInicial.Data = DateTime.Now.AddMonths(-3);
                ctrlDataEmissaoFinal.Data = DateTime.Now;
            }
        }
    }
}
