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
                    lblMsgRestricaoCancelamento.Text = "Só é possível cancelar uma nota fiscal que tenha sido autorizada no período 02 horas após a emissão";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold=true;
                    break;
                case "PR":
                case "RS":
                    lblMsgRestricaoCancelamento.Text = "Só é possível cancelar uma nota fiscal que tenha sido autorizada no período 168 horas(7 Dias) após a emissão";
                    lblMsgRestricaoCancelamento.ForeColor = System.Drawing.Color.Red;
                    lblMsgRestricaoCancelamento.Font.Bold = true;
                    break;
                default:
                    lblMsgRestricaoCancelamento.Text = "Só é possível cancelar uma nota fiscal que tenha sido autorizada no período de 24 horas";
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
