using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;


namespace Glass.UI.Web
{
    public partial class Layout : System.Web.UI.MasterPage
    {
        private const string TITULO_INICIO = "WebGlass :: ";
    
        public bool ExibeMenu
        {
            get { return linhaMenu.Visible; }
            set
            {
                linhaMenu.Visible = value;
    
                if (value)
                    titulo.Style.Remove("border-top");
                else
                    titulo.Style.Add("border-top", "1px solid #E2E2E4");
            }
        }
    
        public bool ExibeBotoesSistema
        {
            get { return botoesSistema.Visible; }
            set { botoesSistema.Visible = value; }
        }
    
        public bool ExibirConfiguracoes
        {
            get { return lnkConfiguracao.Visible; }
            set { lnkConfiguracao.Visible = value; }
        }
    
        private Func<bool> _verificarMensagensNaoLidas = () => false;
    
        public Func<bool> VerificarMensagensNaoLidas
        {
            get { return _verificarMensagensNaoLidas; }
            set { _verificarMensagensNaoLidas = value; }
        }
    
        private string _paginaPrincipal = "~/WebGlass/Main.aspx";
    
        public string PaginaPrincipal
        {
            get { return _paginaPrincipal; }
            set { _paginaPrincipal = value; }
        }
    
        private string _paginaLogin = "~/WebGlass/Default.aspx";
    
        public string PaginaLogin
        {
            get { return _paginaLogin; }
            set { _paginaLogin = value; }
        }

        /// <summary>
        /// Define a observação que aparecerá no menu
        /// </summary>
        public string ObsMenu
        {
            set
            {
                imgObsMenu.Visible = !string.IsNullOrEmpty(value);                
                imgObsMenu.ToolTip = value;
            }
        }

        protected override void AddedControl(Control control, int index)
        {
            if (FuncoesGerais.IsChrome(Page))
                this.Page.ClientTarget = "uplevel";
            
            base.AddedControl(control, index);
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.UserAgent != null && (Request.UserAgent.IndexOf("AppleWebKit") > 0 || Request.UserAgent.IndexOf("Unknown") > 0 || Request.UserAgent.IndexOf("Chrome") > 0))
                Request.Browser.Adapters.Clear();
    
            // Altera o título da página
            if (!Page.Title.Contains(TITULO_INICIO))
            {
                lblTitulo.Text = Page.Title;
                Page.Title = TITULO_INICIO + Page.Title;
            }

            lblTitulo.Text +=
                string.Format("{0}",
                    ControleSistema.AmbienteTeste ?
                        " (AMBIENTE TESTE)" :
                        string.Empty);

            Page.Title +=
                string.Format("{0}",
                    ControleSistema.AmbienteTeste ?
                        " (AMBIENTE TESTE)" :
                        string.Empty);

            // Inicia as threads de controle do sistema, se necessário
            if (!IsPostBack && Page.User.Identity.IsAuthenticated)
                Threads.Instance.IniciarThreads(HttpContext.Current);
    
            // Esconde os itens da tela em caso de popup
            if (IsPopup())
                Page.ClientScript.RegisterStartupScript(GetType(), "popup", "hidePopup();", true);
            
            #region Atualiza acesso e valida login
    
            try
            {
                // Salva numa variável Application o usuário logado, ou atualiza seu último acesso
                UserInfo.SetActivity();
            }
            catch { }
    
            LoginUsuario login = null;
    
            try
            {
                login = UserInfo.GetUserInfo;
            }
            catch { }
    
            if (login == null || (login.CodUser == 0 && login.IdCliente == 0))
            {
                Response.Redirect(PaginaLogin);
                return;
            }
    
            imgCliente.ImageUrl = Logotipo.GetLogoVirtualPath();
    
            if (IsPopup())
                return;
    
            lblUsuario.Text =
                ControleSistema.AmbienteTeste ?
                    "AMBIENTE TESTE" :
                    string.Format("Bem vindo(a), {0}", login.Nome);
    
            if (LojaDAO.Instance.GetCount() > 1)
                lblUsuario.Text += "<br/>" + LojaDAO.Instance.GetNome(login.IdLoja);

            /* Chamado 53668. */
            if (Geral.SistemaLite)
            {
                lblTelSuporte.Visible = false;
                lblCoord.Visible = false;
            }
            else if (!login.IsAdministrador)
                lblCoord.Visible = false;
    
            DateTime dataTrabalho = FuncionarioDAO.Instance.ObtemDataAtraso(login.CodUser);
            lblDataTrabalho.Text = dataTrabalho != DateTime.Now ? " (data de trabalho: " + dataTrabalho.ToString("dd/MM/yyyy") + ")" : "";
    
            // Verifica se há novas mensagens e quantas são
            bool msgNova = VerificarMensagensNaoLidas();
            lnkMensagens.Visible = !msgNova;
            lnkMensagensNaoLidas.Visible = msgNova;

            try
            {
                // Chamado 13112.
                // É necessário informar ao funcionário, financeiro recebimento, quantos pedidos estão aguardando sua confirmação/finalização.
                VerificarPedidosAguardandoFinanceiro();

                VerificarEstoqueMinimo();

                VerificarAvaliacaoPendente();
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Layout.master.cs - Page_Load", ex);
            }

            #endregion

            lnkVersao.Text = GetVersion();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Page.Validators.Count > 0)
            {
                foreach (IValidator validator in Page.Validators)
                {
                    if (validator is Web.Process.Behaviors.BehaviorValidator)
                        Page.ClientScript.RegisterStartupScript(typeof(string), Guid.NewGuid().ToString(),
                            "alert('" + FormatMessage(validator.ErrorMessage, false) + "');", true);
                }
            }

            base.OnPreRender(e);
        }

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            UserInfo.ClearActivity();

            Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>().RemoveMenuFuncMemoria((int)UserInfo.GetUserInfo.CodUser);

            Session.Abandon();
            FormsAuthentication.SignOut();
            Response.Redirect(PaginaLogin);
        }
    
        protected void lnkMensagens_Click(object sender, EventArgs e)
        {
            Response.Redirect(PaginaPrincipal);
        }
    
        protected void lnkConfiguracao_Load(object sender, EventArgs e)
        {
            if (UserInfo.GetUserInfo != null)
                lnkConfiguracao.Visible = UserInfo.GetUserInfo.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Administrador;
            else
                lnkConfiguracao.Visible = false;
        }
    
        protected string GetVersion()
        {
            return "v" + Geral.ObtemVersao();
        }
    
        public bool IsPopup()
        {
            return Request["popup"] == "true";
        }
    
        public bool IsPopupControle()
        {
            return IsPopup() && Request["controlePopup"] == "true";
        }

        protected void btnAtualizarPagina_Click(object sender, EventArgs e)
        {
            Queue<Control> controles = new Queue<Control>();
    
            foreach (Control c in Pagina.Controls)
                controles.Enqueue(c);
    
            while (controles.Count > 0)
            {
                var c = controles.Dequeue();

                if (c is GridView || c is DetailsView || c is DataSourceControl)
                {
                    if (!(c.Parent is RepeaterItem))
                        c.DataBind();
                }

                else if (c.Controls.Count > 0)
                    foreach (Control c1 in c.Controls)
                        controles.Enqueue(c1);
            }
        }

        /// <summary>
        /// Formata uma mensagem para ser usada no JavaScript.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="removerQuebraLinha"></param>
        /// <returns></returns>
        private static string FormatMessage(string message, bool removerQuebraLinha)
        {
            return message.Replace("\"", String.Empty).Replace("'", String.Empty).Replace("\n ", "\n").
                Replace("\n", removerQuebraLinha ? " " : "\\n").Replace("\r", String.Empty);
        }

        /// <summary>
        /// Chamado 13112. Informa aos funcionários de financeiro recebimento quantos pedidos estão aguardando a finalização/confirmação.
        /// </summary>
        public void VerificarPedidosAguardandoFinanceiro()
        {
            var usuario = UserInfo.GetUserInfo;

            if (usuario != null)
            {
                var qtdPedidosAguardandoFinanceiro = PedidoDAO.Instance.ObtemQtdPedidosFinanceiro();

                var exibirAguardandoFinanc = usuario.IsFinanceiroReceb && qtdPedidosAguardandoFinanceiro > 0;

                lblPedidosAguardandoFinanceiro.Visible = exibirAguardandoFinanc;
                lblPedidosAguardandoFinanceiro.Text = "Existem " + qtdPedidosAguardandoFinanceiro + " pedido(s) aguardando a confirmação/" +
                    "finalização pelo financeiro";
            }
        }

        /// <summary>
        /// Informa ao usuário caso tenha avaliação pendente
        /// </summary>
        public void VerificarAvaliacaoPendente()
        {
            var usuario = UserInfo.GetUserInfo;

            if (usuario != null)
            {
                var exibirAvaliacao = AvaliacaoAtendimentoDAO.Instance.PossuiAvaliacaoPendente();

                lnkAvaliacoesPendentes.Visible = exibirAvaliacao;
            }
        }

        /// <summary>
        /// Informa a quantidade de produtos que estão abaixo do ou no estoque mínimo.
        /// </summary>
        public void VerificarEstoqueMinimo()
        {
            var usuario = UserInfo.GetUserInfo;

            if (usuario != null &&
                EstoqueConfig.ExibirQuantidadeProdutosAbaixoOuNoEstoqueMinimoTopoTela &&
                (Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente) ||
                Config.PossuiPermissao(Config.FuncaoMenuEstoque.ControleEstoque)))
            {
                var quantidadeProdutosEstoqueMinimo = ProdutoLojaDAO.Instance.ObtemQuantidadeProdutosAbaixoOuNoEstoqueMinimo((int)usuario.IdLoja);

                if (quantidadeProdutosEstoqueMinimo > 0)
                {
                    lblQuantidadeProdutosEstoqueMinimo.Visible = true;
                    lblQuantidadeProdutosEstoqueMinimo.Text =
                        string.Format("Existe(m) {0} produto(s) abaixo do/no estoque mínimo", quantidadeProdutosEstoqueMinimo);
                }
                else
                    lblQuantidadeProdutosEstoqueMinimo.Visible = false;
            }
            else
                lblQuantidadeProdutosEstoqueMinimo.Visible = false;
        }

        protected void lnkAbrirChamado_Click(object sender, EventArgs e)
        {
            ExibirTelaChamados();
        }

        /// <summary>
        /// Criptografa dados e exibe tela de abertura de chamados
        /// </summary>
        public void ExibirTelaChamados()
        {
            var crypto = new Glass.Seguranca.Crypto(Seguranca.CryptoProvider.Rijndael);
            crypto.Key = "g$1s73EmA4*!><@!(zH))tgf[}6v8/c9";

            var funcionario = FuncionarioDAO.Instance.GetElement(UserInfo.GetUserInfo.CodUser);

            if (string.IsNullOrEmpty(funcionario.Cpf))
                MensagemAlerta.ErrorMsg("É necessário que o funcionário possua cpf cadastrado para prosseguir.",
                    new Exception(), Page);

            var cpf = crypto.Encrypt(Formatacoes.LimpaCpfCnpj(funcionario.Cpf));
            var nomeEmpresa = crypto.Encrypt(System.Configuration.ConfigurationManager.AppSettings["sistema"].ToString());

            // Brasilia/BRA
            var kstZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var horarioBrasilia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kstZone);

            var data = crypto.Encrypt(horarioBrasilia.AddMinutes(15).ToString().Replace(" ", ";"));

            var site = string.Format("{0}?c1={1}&c2={2}&c3={3}", System.Configuration.ConfigurationManager.AppSettings["SiteSuporte"].ToString(),
                data, cpf, crypto.Encrypt(System.Configuration.ConfigurationManager.AppSettings["sistema"]));

            //var site = string.Empty;

            //site = $"{System.Configuration.ConfigurationManager.AppSettings["SiteSuporte"].ToString()}?c1={data}&c2={cpf}&c3={nomeEmpresa}";                

            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "msg",
                string.Format("openWindow(860, 900, '{0}')", site), true);
        }

        protected void divChat_Load(object sender, EventArgs e)
        {
            //divChat.Visible = AbrirChamado && !UserInfo.GetUserInfo.IsCliente;
            /* Chamado 45168. */
            divChat.Visible = !IsPopup() && !UserInfo.GetUserInfo.IsCliente;
        }

        public string ObterNomeUsuario()
        {
            return string.Format("{0} ({1})", UserInfo.GetUserInfo.Nome, System.Configuration.ConfigurationManager.AppSettings["sistema"]);
        }

        public string ObterEmailUsuario()
        {
            return FuncionarioDAO.Instance.GetEmail(UserInfo.GetUserInfo.CodUser);
        }
    }
}
