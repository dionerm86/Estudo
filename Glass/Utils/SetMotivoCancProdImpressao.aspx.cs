using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancProdImpressao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idProdImpressao = Glass.Conversoes.StrParaUint(Request["idProdImpressao"]);
    
            try
            {
                ImpressaoEtiquetaDAO.Instance.CancelarImpressaoComTransacao(UserInfo.GetUserInfo.CodUser, 0, null, null, null, idProdImpressao, txtMotivo.Text, true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok",
                    "alert('Produto impresso cancelado.'); window.opener.atualizarPagina(); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar produto impresso.", ex, Page);
            }
        }
    }
}
