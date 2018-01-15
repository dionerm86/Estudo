using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstDescontoFormaPagtoDadosProduto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void grdDescontoFormaPagtoDadosProduto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "AtivarInativar":
                    DescontoFormaPagamentoDadosProdutoDAO.Instance.AtivarInativar(e.CommandArgument.ToString().StrParaUint());
                    grdDescontoFormaPagtoDadosProduto.DataBind();
                    break;
            }
        }
    }
}