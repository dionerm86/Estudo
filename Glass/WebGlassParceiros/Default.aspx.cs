using System;
using Glass.Data.Helper;
using System.Web.Security;
using Glass.Data.DAL;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = Glass.Configuracoes.Logotipo.GetLogoVirtualPath();
        }
    
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string connString = null;
            LoginUsuario login = null;
    
            try
            {
                login = ClienteDAO.Instance.Autenticacao(txtUsuario.Text, txtSenha.Text, connString);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                return;
            }
    
            FormsAuthentication.RedirectFromLoginPage(login.IdCliente.ToString() + "|cliente", false);
            Response.Redirect("Main.aspx");
        }
    }
}
