using System;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class CentroCustosMes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtAno.Text = DateTime.Now.Year.ToString();
            }
        }
    }
}