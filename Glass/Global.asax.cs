using System;
using System.Web;
using System.Web.Optimization;

namespace Glass.UI.Web
{
    public class Global : System.Web.HttpApplication
    {
        private static bool _configurouCalcEngine = false;
        private static DateTime _dataInicioSistema;

        protected void Application_Start(object sender, EventArgs e)
        {
            new Bootstrapper().Run();

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Code.Tarefas.Agendador.IniciarCopiaContasPagarReceber();

            _dataInicioSistema = DateTime.Now;

            #region CalcEngine

            if (!_configurouCalcEngine)
            {
                try
                {
                    // Carrega os arquivos de configuração
                    var diretorioForvet = Glass.Data.Helper.Utils.ArqConfigForvetPath(this.Context);

                    if (System.IO.File.Exists(diretorioForvet + "config.xml") &&
                        System.IO.File.Exists(diretorioForvet + "machine.xml") &&
                        System.IO.File.Exists(diretorioForvet + "toolStd.cfg") &&
                        System.IO.File.Exists(diretorioForvet + "toolExt.cfg"))
                    {
                        _configurouCalcEngine = true;

                        var arqConfiguracao = diretorioForvet + "config.xml";
                        var arqConfiguracaoMaquina = diretorioForvet + "machine.xml";
                        var arqConfiguracaoFerramentaPadrao = diretorioForvet + "toolStd.cfg";
                        var arqConfiguracaoFerramentas = diretorioForvet + "toolExt.cfg";

                        // Carrega a configuração para os projetos FML
                        CalcEngine.Forvet.FmlProject.Configure(arqConfiguracaoMaquina, arqConfiguracao, arqConfiguracaoFerramentaPadrao, arqConfiguracaoFerramentas);
                    }

                    //Configura o arquivos da Biesse
                    Data.Helper.ConfiguracaoBiesse.Instancia.Inicializar(Data.Helper.Utils.ArqConfigIntermacPath(Context));
                }
                catch
                {
                    _configurouCalcEngine = false;
                }
            }

            #endregion
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            int tempo = 0;

            if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["TempoTimeOut"], out tempo))
                Session.Timeout = tempo;
            else
                Session.Timeout = 20;

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            var cookie = HttpContext.Current.Request.Cookies.Get(System.Web.Security.FormsAuthentication.FormsCookieName);

            if (cookie != null)
            {
                var ticket = System.Web.Security.FormsAuthentication.Decrypt(cookie.Value);

                if (ticket != null &&
                    ticket.IssueDate < _dataInicioSistema &&
                    !StringComparer.InvariantCultureIgnoreCase.Equals(HttpContext.Current.Request.Url.Host, "localhost"))
                {
                    System.Web.Security.FormsAuthentication.SignOut();
                    System.Web.Security.FormsAuthentication.RedirectToLoginPage();
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = HttpContext.Current;

            if (StringComparer.InvariantCultureIgnoreCase.Equals(httpContext.Request.Url.Host, "localhost"))
                return;

            var erro = httpContext.Server.GetLastError();

            if (HttpContext.Current.Session == null)
            {
                /*Response.Redirect(
                    string.Format("~/Utils/Erro.aspx?url={0}&messagem={1}",
                        HttpUtility.HtmlEncode(httpContext.Request.Url.ToString()),
                        HttpUtility.HtmlEncode((erro.Message ?? "").Length > 255 ? erro.Message.Substring(0, 255) : erro.Message)));*/

                return;
            }
            else
            {
                httpContext.Session["UrlErro"] = httpContext.Request.Url.ToString();
                httpContext.Session["Erro"] = httpContext.Server.GetLastError();
                httpContext.Server.ClearError();
                Response.Redirect("~/Utils/Erro.aspx");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            uint? idFunc = Session["idUsuario"] as uint?;
            if (idFunc > 0)
                Glass.Data.DAL.LoginSistemaDAO.Instance.Sair(idFunc.Value, "N/D", false);
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
