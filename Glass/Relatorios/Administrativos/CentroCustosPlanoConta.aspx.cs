using System;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class CentroCustosPlanoConta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ctrlDataIni.Data = DateTime.Now.AddDays(-30);
                ctrlDataFim.Data = DateTime.Now;
            }
        }
    }
}