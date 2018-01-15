using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ProducaoFornoResumo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataIni.Data = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                ctrlDataFim.Data = ctrlDataIni.Data.AddMonths(1).AddDays(-1);
    
                ddlSetor.Items.Add(new ListItem("Todos", "0"));
                ddlSetor.DataBind();
            }
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProducaoFornoResumo.PageIndex = 0;
        }
    }
}
