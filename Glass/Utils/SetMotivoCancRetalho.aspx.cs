using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancRetalho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idRetalhoProducao = Glass.Conversoes.StrParaUint(Request["idRetalhoProducao"]);
    
            try
            {
                RetalhoProducaoDAO.Instance.CancelarComTransacao(UserInfo.GetUserInfo.CodUser, idRetalhoProducao, txtMotivo.Text, true, true, true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok",
                    "alert('Retalho cancelado.'); window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar retalho.", ex, Page);
            }
        }
    }
}
