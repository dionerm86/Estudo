using System;
using System.IO;
using Glass.Data.DAL;
using System.Security.Cryptography.X509Certificates;

namespace Glass.UI.Web.Utils
{
    public partial class SetCertificado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se a situação do certificado digital ainda não tiver sido informada ao usuário, informa
            if (lblSituacao.Text == String.Empty)
            {
                string path = Data.Helper.Utils.GetCertPath + "loja" + Request["idLoja"] + ".pfx";
    
                if (File.Exists(path))
                {
                    lblSituacao.ForeColor = System.Drawing.Color.Green;
                    lblSituacao.Text = "Há um certificado digital cadastrado para esta loja. Dados do Certificado: ";
    
                    try
                    {
                        X509Certificate2 cert = new X509Certificate2();
                        byte[] rawData;
    
                        // Cria certificado
                        using (FileStream f = File.Open(path, FileMode.Open, FileAccess.Read))
                        {
                            f.Position = 0;
                            rawData = new byte[f.Length];
                            f.Read(rawData, 0, rawData.Length);
                        }
    
                        cert.Import(rawData, LojaDAO.Instance.RecuperaSenhaCert(Glass.Conversoes.StrParaUint(Request["idLoja"])), X509KeyStorageFlags.MachineKeySet);
    
                        if (cert.SubjectName != null)
                            lblSituacao.Text += " <br /><br />Empresa: " + cert.SubjectName.Name.Split(',')[0].Replace("CN=", "") + ".";
    
                        if (cert.NotAfter != null)
                            lblSituacao.Text += " Válido até " + cert.NotAfter.ToString("dd/MM/yyyy") + ".";
    
                    }
                    catch (Exception ex) 
                    {
                        Glass.MensagemAlerta.ErrorMsg("Falha ao ler informações do certificado.", ex, Page);
                        return;
                    }
                }
                else
                {
                    lblSituacao.ForeColor = System.Drawing.Color.Red;
                    lblSituacao.Text = "Não há nenhum certificado digital cadastrado para esta loja.";
                }
            }
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            // Verifica se algum arquivo foi selecionado
            if (!fluFoto.HasFile)
            {
                Glass.MensagemAlerta.ShowMsg("Nenhum arquivo selecionado.", Page);
                return;
            }
    
            if (txtSenhaCert.Text != txtConfSenha.Text)
            {
                Glass.MensagemAlerta.ShowMsg("A confirmação da senha está diferente da senha informada.", Page);
                return;
            }
    
            try
            {
                // Salva o arquivo do certificado
                string path = Data.Helper.Utils.GetCertPath + "loja" + Request["idLoja"] + ".pfx";
    
                // Se o arquivo já existir, apaga
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
    
                fluFoto.SaveAs(path);
                ClientScript.RegisterClientScriptBlock(typeof(string), "reload", "alert('Certificado Digital cadastrado.'); closeWindow();", true);
    
                // Atualiza senha no cadastro de lojas
                LojaDAO.Instance.AtualizaSenhaCert(Glass.Conversoes.StrParaUint(Request["idLoja"]), txtSenhaCert.Text);
    
    
                //Recupera os dados do certificado para salvar a data de vencimento
                try
                {
                    X509Certificate2 cert = new X509Certificate2();
                    byte[] rawData;
    
                    // Cria certificado
                    using (FileStream f = File.Open(path, FileMode.Open, FileAccess.Read))
                    {
                        f.Position = 0;
                        rawData = new byte[f.Length];
                        f.Read(rawData, 0, rawData.Length);
                    }
    
                    cert.Import(rawData, LojaDAO.Instance.RecuperaSenhaCert(Glass.Conversoes.StrParaUint(Request["idLoja"])), X509KeyStorageFlags.MachineKeySet);
    
                    string dataVencimento = cert.NotAfter.ToString("dd/MM/yyyy");
                    LojaDAO.Instance.SalvaDataVencimento(Glass.Conversoes.StrParaUint(Request["idLoja"]), DateTime.Parse(dataVencimento));
                }
                catch (Exception ex)
                {
                    Glass.MensagemAlerta.ErrorMsg("Falha ao ler informações do certificado.", ex, Page);
                    return;
                }
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg("Falha ao cadastrar certificado.", ex, Page);
                return;
            }
        }
    }
}
