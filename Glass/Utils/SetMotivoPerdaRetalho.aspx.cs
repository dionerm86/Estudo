using System;
using System.Web.UI;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoPerdaRetalho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idRetalhoProducao = Glass.Conversoes.StrParaUint(Request["idRetalhoProducao"]);
    
            try
            {
                RetalhoProducaoDAO.Instance.MarcarPerda(UserInfo.GetUserInfo.CodUser, idRetalhoProducao, ctrlTipoPerda1.IdTipoPerda.GetValueOrDefault(0),
                    ctrlTipoPerda1.IdSubtipoPerda, txtMotivo.Text);

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok",
                    "alert('Marcado a perda no retalho.'); window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao marcar perda no retalho.", ex, Page);
            }
        }
    }
}
