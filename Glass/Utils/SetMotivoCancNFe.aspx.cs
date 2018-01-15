using System;
using System.Web.UI;
using Glass.Data.NFeUtils;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancNFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = EnviaXML.EnviaCancelamentoEvt(Glass.Conversoes.StrParaUint(Request["idNf"]), txtMotivo.Text);
    
                Glass.MensagemAlerta.ShowMsg(msg, Page);

                Page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                "window.close();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar NFe.", ex, Page);
                return;
            }
        }
    }
}
