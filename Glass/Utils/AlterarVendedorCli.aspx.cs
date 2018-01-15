using System;
using System.Web.UI;

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
                var idVendedor = Glass.Conversoes.StrParaUintNullable(drpFuncionario.SelectedValue);
                
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ok", String.Format(@"
                    window.opener.alteraVendedor('{0}');
                    closeWindow();", idVendedor), true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao alterar vendedor.", ex, Page);
            }
        }
    }
}
