using System;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaProducaoInst : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (((TextBox)ctrlDataIni.FindControl("txtData")).Text == String.Empty)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("01/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }
    }
}
