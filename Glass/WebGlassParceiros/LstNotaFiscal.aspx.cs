using System;
using System.Web.UI;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class LstNotaFiscal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["erroNf"] != null)
                ClientScript.RegisterClientScriptBlock(GetType(), "erroNf", "alert('O arquivo dessa nota fiscal não foi encontrado.')", true);
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdNf.DataBind();
            grdNf.PageIndex = 0;
        }
    }
}
