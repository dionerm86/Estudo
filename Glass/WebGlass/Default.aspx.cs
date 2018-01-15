using System;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.Web.Security;
using System.Web;

namespace Glass.UI.Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imgLogo.ImageUrl = Configuracoes.Logotipo.GetLogoVirtualPath();

            if (!Data.Helper.Utils.IsLocalUrl(HttpContext.Current) && !string.IsNullOrEmpty(Request["token"]))
            {
                var usuarioSync = Seguranca.AutenticacaoAutomatica.Autenticar(Request["token"], System.Configuration.ConfigurationManager.AppSettings["sistema"]);
                var idAdmin = FuncionarioDAO.Instance.ObterIdAdminSync();

                if (string.IsNullOrEmpty(usuarioSync))
                {
                    MensagemAlerta.ShowMsg("Falha ao descriptografar token.", Page);
                    return;
                }
                else if (usuarioSync.Split('|')[0] == "Erro")
                {
                    MensagemAlerta.ShowMsg(usuarioSync.Split('|')[1], Page);
                    return;
                }

                if (idAdmin == 0)
                {
                    MensagemAlerta.ShowMsg("Nenhum admin sync cadastrado.", Page);
                    return;
                }

                var dadosLogin = string.Format("{0}||{1}", idAdmin, usuarioSync);

                FormsAuthentication.RedirectFromLoginPage(dadosLogin, false);
                Response.Redirect("Main.aspx", false);
            }
        }
    
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            LoginUsuario login = new LoginUsuario();
    
            try
            {
                login = FuncionarioDAO.Instance.Autenticacao(txtUsuario.Text, txtSenha.Text);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ShowMsg(ex.Message, Page);
                return;
            }
    
            FormsAuthentication.RedirectFromLoginPage(login.CodUser.ToString(), false);
            Response.Redirect("Main.aspx", false);
        }
    }
}