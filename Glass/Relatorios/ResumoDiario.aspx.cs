using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ResumoDiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
