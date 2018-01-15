using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoForno : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Today.ToString("dd/MM/yyyy");
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProducaoForno.PageIndex = 0;
        }
    
        protected void lblNomePrimSetor_PreRender(object sender, EventArgs e)
        {
            ((Label)sender).Text = SetorDAO.Instance.GetNomePrimSetor();
        }
    }
}
