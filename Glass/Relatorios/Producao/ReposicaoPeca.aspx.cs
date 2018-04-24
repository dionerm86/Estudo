using Glass.Data.Helper;
using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios.Producao
{
    public partial class ReposicaoPeca : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof (MetodosAjax));

            if (!IsPostBack)
            {
                ctrlDataIni.Data = DateTime.Today.AddMonths(-1);
                ctrlDataFim.Data = DateTime.Today;
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdPerda.PageIndex = 0;
        }
    }
}
