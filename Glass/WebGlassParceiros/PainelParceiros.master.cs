using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class PainelParceiros : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoginUsuario login = null;
    
            var l = Page.Master.Master as Glass.UI.Web.Layout;
            l.ExibirConfiguracoes = false;
            l.ExibeBotoesSistema = true;
            l.ExibeMenu = true;
            l.VerificarMensagensNaoLidas = () => false;
            l.PaginaPrincipal = "~/WebGlassParceiros/Main.aspx";
            l.PaginaLogin = "~/WebGlassParceiros/Default.aspx";
    
            //if (!IsPostBack)
            {
                try
                {
                    login = UserInfo.GetUserInfo;
                }
                catch { }
            }
    
            #region Cria��o do Menu
    
            //if (!IsPostBack)
            {    
                // Define o menu de altera��o de senha
                if (login != null)
                    mnuParc.FindItem("ALTERAR SENHA").NavigateUrl += "?idCli=" + login.IdCliente.Value;

                mnuParc.FindItem("ALTERAR SENHA").NavigateUrl = "javascript:openWindow(150, 310, '" + ResolveUrl(mnuParc.FindItem("ALTERAR SENHA").NavigateUrl) + "');";
                mnuParc.FindItem("ALTERAR SENHA").Target = "_blank";
    
                // Passa os itens principais do menu para letra mai�scula
                foreach (MenuItem m in mnuParc.Items)
                    m.Text = m.Text.ToUpper();
            }        
    
            #endregion

            #region Itens do Menu
 
            if (Configuracoes.MenuConfig.ExibirConsultaProducaoECommerce)
                mnuParc.Items.AddAt(4, new MenuItem("CONSULTAR PRODU��O", "CONSULTAR PRODU��O", "", "~/WebGlassParceiros/lstProducao.aspx"));

            if (Configuracoes.MenuConfig.ExibiPrecoTabelaECommerce)
                mnuParc.Items.Add(new MenuItem("PRE�OS DE TABELA", "PRE�OS DE TABELA", "", "~/WebGlassParceiros/ListaPrecoTabCliente.aspx"));

            if(Configuracoes.ProjetoConfig.SelecionarEspessuraAoCalcularProjeto && Configuracoes.ProjetoConfig.ExibirFolgaProjetoEcommerce)
                mnuParc.Items.Add(new MenuItem("CONFIGURAR FOLGAS", "CONFIGURAR FOLGAS", "", "~/WebGlassParceiros/CadPecaModelo.aspx"));

            #endregion
        }
    
        public bool IsPopup()
        {
            return (Page.Master.Master as Glass.UI.Web.Layout).IsPopup();
        }
    }
}
