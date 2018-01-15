using System;
using System.Web.UI;

namespace Glass.UI.Web.Listas
{
    public partial class LstRegistroArquivoRemessa : System.Web.UI.Page
    {
        #region Variaveis locais
    
        private bool corAlternada = true;
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
    
        }
    }
}
