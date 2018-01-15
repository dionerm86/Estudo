using System;
using Glass.Data.CTeUtils;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoInutCTe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = EnviaXML.EnviaInutilizacao(Glass.Conversoes.StrParaUint(Request["idCte"]), txtMotivo.Text);
    
                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("falha ao inutilizar cte.", ex, Page);
                return;
            }
        }
    }
}
