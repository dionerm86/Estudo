using System;
using System.Web.UI;
using Glass.Data.NFeUtils;

namespace Glass.UI.Web.Utils
{
    public partial class SetMotivoInutNFe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
    
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var idNf = Conversoes.StrParaUint(Request["idNf"]);
                var idLoja = Glass.Data.DAL.NotaFiscalDAO.Instance.ObtemIdLoja(idNf);

                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado(idLoja);

                if (DateTime.Now > cert.NotAfter)
                    throw new Exception("O certificado digital cadastrado est� vencido, insira um novo certificado para emitir esta nota. Data Venc.: " + cert.GetExpirationDateString());

                string msg = EnviaXML.EnviaInutilizacao(idNf, txtMotivo.Text);

                MensagemAlerta.ShowMsg(msg, Page);

                Page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                "window.close();", true);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao inutilizar NFe.", ex, Page);
                return;
            }
        }
    }
}
