using Glass.Data.DAL;
using System;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancDepositoCheque : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                DepositoChequeDAO.Instance.CancelarDeposito(Request["idDeposito"].StrParaUint(), txtMotivo.Text, ctrlDataEstorno.DataNullable, chkEstornar.Checked);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
                return;
            }

            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href);closeWindow();", true);
        }
    }
}