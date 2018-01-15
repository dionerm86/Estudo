using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SelTextoOrca : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Page.Title += Request["idOrca"];
        }
    
        protected void grdTextoImprOrca_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "IncluiTexto")
            {
                TextoOrcamento textoOrca = new TextoOrcamento();
                textoOrca.IdOrcamento = Glass.Conversoes.StrParaUint(Request["idOrca"]);
                textoOrca.IdTextoImprOrca = Glass.Conversoes.StrParaUint(e.CommandArgument.ToString());
                TextoOrcamentoDAO.Instance.Insert(textoOrca);
    
                grdTextoOrca.DataBind();
            }
        }
    }
}
