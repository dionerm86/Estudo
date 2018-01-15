using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                var idRota = Glass.Conversoes.StrParaUintNullable(drpRota.SelectedValue);

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", String.Format(@"
                    window.opener.alterarRota('{0}');
                    closeWindow();", idRota), true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao alterar rota.", ex, Page);
            }
        }
    }
}