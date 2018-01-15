using System;
using System.Web.UI;
using Glass.Data.NFeUtils;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoCancNFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var idNotaFiscal = Request["IdNf"];

            var estadoLoja = "";

            var idLoja = NotaFiscalDAO.Instance.GetElement(idNotaFiscal.StrParaUint()).IdLoja;

            if (idLoja.GetValueOrDefault() > 0 )
                estadoLoja = LojaDAO.Instance.GetElement(idLoja.Value).Uf;

            switch (estadoLoja.ToUpper())
            {
                case "MT":
                    lblMsgRestricaoCancelamento.Text = "S� � poss�vel cancelar uma nota fiscal que tenha sido autorizada no per�odo 02 horas ap�s a emiss�o";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold=true;
                    break;
                case "PR":
                case "RS":
                    lblMsgRestricaoCancelamento.Text = "S� � poss�vel cancelar uma nota fiscal que tenha sido autorizada no per�odo 168 horas(7 Dias) ap�s a emiss�o";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold = true;
                    break;
                default:
                    lblMsgRestricaoCancelamento.Text = "S� � poss�vel cancelar uma nota fiscal que tenha sido autorizada no per�odo de 24 horas";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold = true;
                    break;
            }
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
