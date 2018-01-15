using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstOtimizacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdOtimizacao.ShowFooter = false;
                grdOtimizacao.PageSize = 10;
            }
        }

        protected void lnkInserirOtmizacao_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadOtimizacao.aspx?aluminio=1");
        }
    }
}