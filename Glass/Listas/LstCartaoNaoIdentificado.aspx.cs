using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstCartaoNaoIdentificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdCartaoNaoIdentificado.DataBind();
        }
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadCartaoNaoIdentificado.aspx" + (!string.IsNullOrEmpty(Request["cxDiario"]) ? "?cxDiario=1" : ""));
        }
        protected void lnkImportar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/ImportarCartaoNaoIdentificado.aspx" + (!string.IsNullOrEmpty(Request["cxDiario"]) ? "?cxDiario=1" : ""));
        }
        protected void lnkArquivos_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Utils/LstArquivoCartaoNaoIdentificado.aspx");
        }
    }
}
