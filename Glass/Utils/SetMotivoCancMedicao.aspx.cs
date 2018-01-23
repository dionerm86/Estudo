using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancMedicao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                MedicaoDAO.Instance.CancelarMedicao(Request["IdMedicao"].StrParaIntNullable().GetValueOrDefault(), txtMotivo.Text);

                ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }
        }
    }
}
