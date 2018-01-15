using System;
using System.Web.UI;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaEntregaRota : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDataIni.Data = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                txtDataFim.Data = DateTime.Now;
            }
    
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    
        protected void grdDados_DataBound(object sender, EventArgs e)
        {
            if (grdDados.Rows.Count > 0)
                parametrosRelatorio.Visible = true;
            else
                parametrosRelatorio.Visible = false;
        }
    }
}
