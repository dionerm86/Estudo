using Glass.Data.MDFeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancMDFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsgRestricaoCancelamento.Text = @"O prazo de cancelamento do MDFe é de 24 horas. 
                                                        Obs: Não é possível cancelar MDFe que já tenha iniciado a viagem. ";
            lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
            lblMsgRestricaoCancelamento.Font.Bold = true;
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = EnviaXML.EnviaCancelamento(Glass.Conversoes.StrParaInt(Request["IdMDFe"]), txtMotivo.Text);

                Glass.MensagemAlerta.ShowMsg(msg, Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cancelar MDFe.", ex, Page);
                return;
            }
        }
    }
}