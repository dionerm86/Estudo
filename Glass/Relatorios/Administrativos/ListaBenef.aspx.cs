using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class ListaBenef : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime inicio = DateTime.Now.AddMonths(-1);
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = inicio.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = inicio.AddMonths(1).ToString("dd/MM/yyyy");
    
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdBenef.PageIndex = 0;
        }
    }
}
