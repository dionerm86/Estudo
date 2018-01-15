using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Utils
{
    public partial class Erro : System.Web.UI.Page
    {
        #region Propriedades
    
        /// <summary>
        /// Url de retorno.
        /// </summary>
        public string BackUrl { get; set; }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Uri urlErro = null;
            var textoUrl = Session["UrlErro"] as string ?? Request["url"];
    
            // Tenta recupera a url de onde ocorreu o erro
            if (!string.IsNullOrEmpty(textoUrl) && Uri.TryCreate(textoUrl, UriKind.RelativeOrAbsolute, out urlErro))
            {
                url.Visible = UserInfo.GetUserInfo.IsAdminSync;
                lblURL.Text = urlErro.ToString();
                BackUrl = urlErro.ToString();
            }
    
            // Recupera os dados do erro da sessão
            var erro = Session["Erro"] as Exception;
            Session["Erro"] = null;
            
            if (erro == null || erro.InnerException == null)
                Response.Redirect("~/Webglass/Main.aspx");
    
            try
            {
                // Tenta recupera a mensage do erro.
                lblErro.Text = ErroDAO.Instance.GetErrorMessage(erro.InnerException ?? erro);
            }
            catch (Exception)
            {
                lblErro.Text = (erro.InnerException ?? erro).Message;
            }
    
            if (urlErro != null)
            {
                try
                {
                    // Insere o erro na tabela
                    ErroDAO.Instance.InserirFromException(urlErro.ToString(), erro);
                }
                catch (Exception ex)
                {
                    lblErro.Text += "Fala ao registrar o erro no sistema: " + ex.Message;
                }
            }
            
            erro = erro.InnerException ?? erro;
            if (erro is Glass.Relatorios.UI.Web.ReportPage.ReportException)
            {
                var re = erro as Glass.Relatorios.UI.Web.ReportPage.ReportException;
                if (!String.IsNullOrEmpty(re.JavaScript))
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "Erro", re.JavaScript, true);
            }
        }
    }
}
