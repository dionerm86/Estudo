using System;
using System.Web.UI;
using Glass.Configuracoes;

namespace Glass.UI.Web.Utils
{
    public partial class ConsultaEnvio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string compl = "E-mail";
    
                email.Visible = true;
                separador.Visible = true;

                if (PCPConfig.EmailSMS.EnviarSMSPedidoPronto || PCPConfig.EmailSMS.EnviarSMSAdministrador)
                {
                    compl += " e ";
                    sms.Visible = true;
                    compl += "SMS";
                }
    
                Page.Title += compl;
            }
        }
    }
}
