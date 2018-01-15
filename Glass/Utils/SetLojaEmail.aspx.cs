using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class SetLojaEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                uint idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
                Loja loja = LojaDAO.Instance.GetElementByPrimaryKey(idLoja);
    
                tbxEmailFiscal.Text = loja.EmailFiscal;
                tbxLoginFiscal.Text = loja.LoginEmailFiscal;
                tbxServidorFiscal.Text = loja.ServidorEmailFiscal;
    
                tbxEmailComercial.Text = loja.EmailComercial;
                tbxLoginComercial.Text = loja.LoginEmailComercial;
                tbxServidorComercial.Text = loja.ServidorEmailComercial;
    
                tbxEmailContato.Text = loja.EmailContato;
                tbxLoginContato.Text = loja.LoginEmailContato;
                tbxServidorContato.Text = loja.ServidorEmailContato;
            }
        }
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            uint idLoja = Glass.Conversoes.StrParaUint(Request["idLoja"]);
            Loja loja = LojaDAO.Instance.GetElementByPrimaryKey(idLoja);
    
            if (!String.IsNullOrEmpty(tbxEmailFiscal.Text))
                loja.EmailFiscal = tbxEmailFiscal.Text;
    
            if (!String.IsNullOrEmpty(tbxLoginFiscal.Text))
                loja.LoginEmailFiscal = tbxLoginFiscal.Text;
    
            if (!String.IsNullOrEmpty(tbxServidorFiscal.Text))
                loja.ServidorEmailFiscal = tbxServidorFiscal.Text;
    
            if (!String.IsNullOrEmpty(tbxSenhaFiscal.Text))
                loja.SenhaEmailFiscal = tbxSenhaFiscal.Text;
    
            if (!String.IsNullOrEmpty(tbxEmailComercial.Text))
                loja.EmailComercial = tbxEmailComercial.Text;
    
            if (!String.IsNullOrEmpty(tbxLoginComercial.Text))
                loja.LoginEmailComercial= tbxLoginComercial.Text;
    
            if (!String.IsNullOrEmpty(tbxServidorComercial.Text))
                loja.ServidorEmailComercial = tbxServidorComercial.Text;
    
            if (!String.IsNullOrEmpty(tbxSenhaComercial.Text))
                loja.SenhaEmailComercial = tbxSenhaComercial.Text;
    
            if (!String.IsNullOrEmpty(tbxEmailContato.Text))
                loja.EmailContato = tbxEmailContato.Text;
    
            if (!String.IsNullOrEmpty(tbxLoginContato.Text))
                loja.LoginEmailContato = tbxLoginContato.Text;
    
            if (!String.IsNullOrEmpty(tbxSenhaContato.Text))
                loja.SenhaEmailContato = tbxSenhaContato.Text;
    
            if (!String.IsNullOrEmpty(tbxServidorContato.Text))
                loja.ServidorEmailContato = tbxServidorContato.Text;
    
            try
            {
                LojaDAO.Instance.UpdateDadosEmail(loja);
    
                lblupdate.Text = "Dados de e-mail da loja " + loja.NomeFantasia + "<br />atualizados com sucesso!";
                lblupdate.ForeColor = System.Drawing.Color.Blue;
            }
            catch (Exception)
            {
                lblupdate.Text = "Erro ao atualizar os dados. <br/>Tente novamente.";
                lblupdate.ForeColor = System.Drawing.Color.Red;
            }
    
        }
    }
}
