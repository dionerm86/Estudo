using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadChequeDevolvido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsFinanceiroPagto())
                Page.Title = "Controle de Cheques Próprios Devolvidos";
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCheque.PageIndex = 0;
        }
    
        protected bool IsFinanceiroPagto()
        {
            return Request["pagto"] == "1";
        }
    }
}
