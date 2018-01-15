using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class RelacaoBoxProducao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ((TextBox)ctrlData.FindControl("txtData")).Text = DateTime.Today.ToString("dd/MM/yyyy");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdRelacaoBox.PageIndex = 0;
        }
    }
}
