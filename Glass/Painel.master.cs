using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web
{
    public partial class Painel : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var login = UserInfo.GetUserInfo;

            if (login == null)
            {
                Response.Redirect("~/WebGlass/Default.aspx");
                return;
            }

            if (login.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao || login.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.SupervisorProducao)
            {
                if (!IsPopup())
                    RedirecionaTelasProducao(login, !this.Page.Request.CurrentExecutionFilePath.ToLower().Contains("lstproducao.aspx"));
                return;
            }

            ConfiguraLayoutMaster(login);
            
            CriarMenu(login);
        }

        /// <summary>
        /// Cria o menu do sistema.
        /// </summary>
        private void CriarMenu(LoginUsuario login)
        {
            if (!mnuGeral.Visible || IsPopup())
                return;

            try
            {
                // Recupera o funcion�rio
                var funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>().ObtemFuncionario((int)login.CodUser);

                if (funcionario == null)
                {
                    Response.Redirect("~/WebGlass/Default.aspx");
                    return;
                }

                // Recupera os menus do sistema
                var fluxoMenu = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>();
                var menusFunc = fluxoMenu.ObterMenusPorFuncionario(funcionario);

                if (menusFunc == null || menusFunc.Count() == 0)
                {
                    Response.Redirect("~/WebGlass/Default.aspx");
                    return;
                }

                // Verifica se a pessoa tem acesso ao menu, se n�o tiver, redireciona para a tela padr�o
                // Pega todos os menus da empresa e verifica se a p�gina atual est� listada nele, se tiver, verifica se o usu�rio tem acesso � ela
                var paginaAtual = Request.Url.LocalPath.ToLower();
                var idsMenu = fluxoMenu
                    .ObterMenusPorConfig((int)login.IdLoja)
                    .Where(f => !string.IsNullOrEmpty(f.Url) &&
                    paginaAtual.Contains(f.Url.Contains('?') ? //Verifica se a url contem QueryString
                        f.Url.ToLower().Remove(f.Url.IndexOf('?')).TrimStart('~', '/') :
                        f.Url.ToLower().TrimStart('~', '/')))
                    .Select(f => f.IdMenu).ToList();

                if (idsMenu.Count > 0 && !menusFunc.Any(f => idsMenu.Contains(f.IdMenu)))
                {
                    Response.Redirect("~/WebGlass/Main.aspx");
                    return;
                }

                // Carrega a lista de menus salva em mem�ria, caso n�o tenha, preenche primeiro, salva em mem�ria e monta o menu
                var lstMenu = Config.CarregaMenusUsuario((int)login.CodUser);

                // Preenche tooltip do menu
                if (idsMenu.Count() > 0)
                {
                    var menu = menusFunc.Where(f => f.IdMenu == idsMenu[0]).FirstOrDefault();

                    if (menu != null && menu.Observacao != null)
                        (Page.Master.Master as Layout).ObsMenu = menu.Observacao;
                }

                if (lstMenu.Count == 0)
                {
                    /* Chamado 44710. */
                    var financeiroPossuiSubmenu = menusFunc.Any(f => f.IdMenuPai == 196);

                    /* Chamado 47175. */
                    var financeiroPagamentoPossuiSubmenu = menusFunc.Any(f => f.IdMenuPai == 260);

                    var menus = menusFunc.Where(f => f.IdMenuPai.GetValueOrDefault() == 0 && (financeiroPossuiSubmenu ? true : f.IdMenu != 196) && (financeiroPagamentoPossuiSubmenu ? true : f.IdMenu != 260));

                    if (menus == null || menus.Count() == 0)
                        ErroDAO.Instance.InserirFromException("Falha ao carregar menu", new Exception("Nenhum menu foi retornado para o funcionario. Ln.: 81"));

                    PopularMenu(menus, menusFunc, null, lstMenu);
                    Config.SalvaMenuUsuario((int)login.CodUser, lstMenu);
                }

                if (lstMenu == null || lstMenu.Count == 0)
                    ErroDAO.Instance.InserirFromException("Falha ao carregar menu", new Exception("O menu n�o foi populado. Ln.: 88"));

                foreach (var menu in lstMenu)
                    mnuGeral.Items.Add(menu);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao carregar menu", ex);
                throw ex;
            }
        }
 
        protected void Page_Unload(object sender, EventArgs e)
        {
            if (mnuGeral.Items.Count == 0 && mnuGeral.Visible && (Page.Master.Master as Glass.UI.Web.Layout).ExibeMenu)
                ErroDAO.Instance.InserirFromException("Falha ao carregar menu", new Exception("O menu n�o foi populado. Page_Unload"));
        }

        /// <summary>
        /// Fun��o recursiva que popula o menu de acordo com as configura��es da empresa e permiss�es que o funcion�rio possui
        /// </summary>
        /// <param name="lstMenuIteracao">Lista com menus que deve rodar no foreach</param>
        /// <param name="lstTodosMenus">Lista com todos os menus que o usu�rio tem acesso</param>
        /// <param name="menuPai"></param>
        private void PopularMenu(IEnumerable<Glass.Global.Negocios.Entidades.Menu> lstMenuIteracao, IEnumerable<Glass.Global.Negocios.Entidades.Menu> lstMenuTodos, MenuItem menuPai, List<MenuItem> lstMenu)
        {
            foreach (var menu in lstMenuIteracao)
            {
                MenuItem menuItem = new MenuItem
                {
                    Text = menu.Nome,
                    Value = menu.IdMenu.ToString(),
                    NavigateUrl = menu.Url
                };

                PopularMenu(lstMenuTodos.Where(f => f.IdMenuPai == menu.IdMenu), lstMenuTodos, menuItem, lstMenu);

                // Se o menu n�o possuir pai, adiciona-o direto no menu, caso contr�rio, inclui o mesmo no seu menu pai
                if (menu.IdMenuPai.GetValueOrDefault() == 0)                    
                    lstMenu.Add(menuItem);
                else
                    menuPai.ChildItems.Add(menuItem);
            }
        }

        /// <summary>
        /// Redirecionamento para telas de ch�o de f�brica
        /// </summary>
        private void RedirecionaTelasProducao(LoginUsuario login, bool redirecionarTelaExpedicao)
        {
            // Controle de produ��o por pedido
            if (login.TipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.SupervisorProducao)
            {
                Response.Redirect("~/Cadastros/CadPedidoCorte.aspx?sit=2");
                return;
            }

            // Obt�m os setores que o funcion�rio possui acesso
            var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(login.CodUser);

            if (OrdemCargaConfig.UsarControleOrdemCarga && funcSetor.Count > 0)
            {
                foreach (var fs in funcSetor)
                {
                    var setor = Data.Helper.Utils.ObtemSetor((uint)fs.IdSetor);
                    if (setor != null && setor.Tipo == TipoSetor.ExpCarregamento)
                    {
                        /* Chamado 39948. */
                        if (!IsPopup() && redirecionarTelaExpedicao)
                        {
                            // A condi��o "Request.UserAgent.IndexOf("MSIE 6.0")" foi criada para que ao entrar no sistema pelo navegador 
                            // de um dispositivo m�vel com windows CE, redirecione para uma tela pr�pria para ele
                            if (Request.Browser.IsMobileDevice || Request.UserAgent.IndexOf("MSIE 6.0") > 0)
                                Response.Redirect("~/Cadastros/Expedicao/CadLeituraCarregamentoMobile.aspx");
                            else
                                Response.Redirect("~/Cadastros/Expedicao/CadLeituraCarregamento.aspx");
                        }

                        return;
                    }
                }
            }

            if (PCPConfig.UsarNovoControleExpBalcao && funcSetor.Count > 0 && !IsPopup())
            {
                foreach (var fs in funcSetor)
                    if (Data.Helper.Utils.ObtemSetor((uint)fs.IdSetor).Tipo == TipoSetor.Entregue)
                    {
                        /* Chamado 39948. */
                        if (!IsPopup() && redirecionarTelaExpedicao)
                        {
                            if (Request.Browser.IsMobileDevice || Request.UserAgent.IndexOf("MSIE 6.0") > 0)
                                Response.Redirect("~/Cadastros/Expedicao/CadLeituraExpBalcaoMobile.aspx");
                            else
                                Response.Redirect("~/Cadastros/Expedicao/CadLeituraExpBalcao.aspx");
                        }

                        return;
                    }
            }

            if (Request.Url.ToString().ToLower().Contains("lstproducao.aspx"))
            {
                mnuGeral.Visible = false;
                Page.ClientScript.RegisterStartupScript(GetType(), "consProd", "hidePopup();", true);
                return;
            }

            if (Request.Url.ToString().ToLower().Contains("trocadev.aspx") &&
                Request.Url.ToString().ToLower().Contains("popup=1"))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "troca", "hidePopup();", true);
                return;
            }

            if (Request.Browser.IsMobileDevice || Request.UserAgent.IndexOf("MSIE 6.0") > 0)
                Response.Redirect("~/Cadastros/Producao/CadProducaoMobile.aspx");
            else
                Response.Redirect("~/Cadastros/Producao/CadProducao.aspx");
        }

        /// <summary>
        /// Configura itens do layout.master
        /// </summary>
        private void ConfiguraLayoutMaster(LoginUsuario login)
        {
            var mensagemFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Glass.Global.Negocios.IMensagemFluxo>();

            Layout l = Page.Master.Master as Layout;
            l.ExibeBotoesSistema = !IsPopup();
            l.ExibeMenu = !IsPopup();
            l.VerificarMensagensNaoLidas = () => !IsPopup() ? mensagemFluxo.ExistemNovasMensagens((int)login.CodUser) : false;
            l.PaginaPrincipal = "~/WebGlass/Main.aspx";
            l.PaginaLogin = "~/WebGlass/Default.aspx";
        }

        public bool IsPopup()
        {
            return (Page.Master.Master as Glass.UI.Web.Layout).IsPopup();
        }

        public bool IsMobileDevice()
        {
            string[] mobileDevices = new string[] {"iphone", "ppc", "windows ce", "blackberry", "opera mini", "mobile", "palm", "portable", "opera mobi" };

            string userAgent = Request.UserAgent.ToString().ToLower();
            return mobileDevices.Any(x => userAgent.Contains(x));
        }
    }
}
