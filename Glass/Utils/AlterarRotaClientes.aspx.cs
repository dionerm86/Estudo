using System;

namespace Glass.UI.Web.Utils
{
    public partial class AlterarRotaClientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAlterarRota_Click(object sender, EventArgs e)
        {
            try
            {
                var idRota = Conversoes.StrParaUintNullable(this.drpRota.SelectedValue);
                var retorno = string.Empty;

                if (this.Request["vue"] == "true")
                {
                    retorno = $"window.opener.app.alterarRota({idRota}); window.close();";
                }
                else
                {
                    retorno = $"window.opener.alterarRota('{idRota}'); closeWindow();";
                }

                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", retorno, true);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao alterar rota.", ex, this.Page);
            }
        }
    }
}