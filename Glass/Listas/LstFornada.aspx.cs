using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Listas
{
    public partial class LstFornada : System.Web.UI.Page
    {
        private bool corAlternada = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                grdFornada.ShowFooter = false;
            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFornada.PageIndex = 0;
        }

        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    }
}