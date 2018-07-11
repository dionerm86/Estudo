using System;

namespace Glass.UI.Web.Utils
{
    public partial class AlterarVendedorCli : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAlterarVendedor_Click(object sender, EventArgs e)
        {
            try
            {
                var idVendedor = Conversoes.StrParaUintNullable(this.drpFuncionario.SelectedValue);
                var retorno = string.Empty;

                if (this.Request["vue"] == "true")
                {
                    retorno = $"window.opener.app.alterarVendedor({idVendedor}); window.close();";
                }
                else
                {
                    retorno = $"window.opener.alteraVendedor('{idVendedor}'); closeWindow();";
                }

                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", retorno, true);
            }
            catch (Exception ex)
            {
                MensagemAlerta.ErrorMsg("Falha ao alterar vendedor.", ex, this.Page);
            }
        }
    }
}
