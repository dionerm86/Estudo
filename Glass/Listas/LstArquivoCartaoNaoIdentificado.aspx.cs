using System;
using System.IO;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstArquivoCartaoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCartaoNaoIdentificado.DataBind();
        }       
    }
}
