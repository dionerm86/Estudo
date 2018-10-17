using System;
using System.Web.UI;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadSubgrupoProduto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.Title.Contains(":"))
                Page.Title += ": " + Request["descrGrupo"];
        }
    }
}
