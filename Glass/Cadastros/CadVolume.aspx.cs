using System;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadVolume : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                //((TextBox)ctrlDataEntIni.FindControl("txtData")).Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
                //((TextBox)ctrlDataEntFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    }
}
