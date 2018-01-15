using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstArquivoCalcEngine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
        }
    
        protected void odsArquivoCalcEngine_Deleted(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar o arquivo.", e.Exception, Page);
                e.ExceptionHandled = true;
            }
            else
                Glass.MensagemAlerta.ShowMsg("Arquivo excluso.", Page);
        }
    }
}
