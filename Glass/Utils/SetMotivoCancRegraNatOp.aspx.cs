using System;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancRegraNatOp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var fluxo = ServiceLocator.Current.GetInstance<Glass.Fiscal.Negocios.ICfopFluxo>();

                var idRegraNaturezaOperacao = Request["Id"].StrParaInt();
                // Carrega os dados da regra
                var regra = fluxo.ObtemRegraNaturezaOperacao(idRegraNaturezaOperacao);
                var resultado = fluxo.ApagarRegraNaturezaOperacao(regra, txtMotivo.Text, true);

                if (!resultado)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao excluir regra de natureza de operação.", resultado);
                    return;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao excluir regra de natureza de operação.", ex, Page);
                return;
            }
    
            ClientScript.RegisterClientScriptBlock(this.GetType(), "ok", "window.opener.redirectUrl(window.opener.location.href); closeWindow();", true);
        }
    }
}
