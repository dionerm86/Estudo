using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Projeto
{
    public partial class ImprimirProjeto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["idProjeto"] != null)
                    Page.Title += " Projeto n.º " + Request["idProjeto"];
    
                else if (Request["idPedido"] != null)
                    Page.Title += " Pedido n.º " + Request["idPedido"];

                else if (Request["idOrcamento"] != null)
                    Page.Title += " Orçamento n.º " + Request["idOrcamento"];
            }
        }
    
        protected void grdItemProjeto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            bool pcp = Request["pcp"] == "1";
            Response.Redirect(ResolveClientUrl("~/Cadastros/Projeto/CadProjetoAvulso.aspx") + e.CommandArgument + (pcp ? "&pcp=1" : "") + "&visualizar=1");
        }
    }
}
