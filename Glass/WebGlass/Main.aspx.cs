using System;
using System.Web.UI;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web
{
    public partial class Main : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdMensagem.Register();
            odsMensagem.Register();
            grdMensagemParceiro.Register();
            odsMensagemParceiro.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.PermitirEnvioMensagemInterna))
            {
                lnkNovaMensagem.Visible = false;
                lnkEnviadas.Visible = false;
            }
            
            lblMensagemFunc.Visible = true;
            lblMensagemParceiro.Visible = true;
            lnkNovaMensagemParceiro.Visible = true;
            grdMensagemParceiro.Visible = true;

            /* Chamado 15992.
             * O usuário informou que as mensagens enviadas não poderiam ficar sem referência de destinatário,
             * portanto, criamos esta configuração para permitir ou não a exclusão de mensagens. */
            grdMensagem.Columns[1].Visible = Glass.Configuracoes.MensagemConfig.TelaMensagens.ExibirBotaoExcluirMensagem;
            grdMensagemParceiro.Columns[1].Visible = Glass.Configuracoes.MensagemConfig.TelaMensagens.ExibirBotaoExcluirMensagem;

            if (!IsPostBack && Glass.Configuracoes.Geral.ExibirAlertasAdministrador && UserInfo.GetUserInfo.IsAdministrador)
            {
                var vencimentoBoleto = ContasReceberDAO.Instance.BoletosVencimentoHoje();
                var pedidosProntosNaoEntregues = PedidoDAO.Instance.ExistemPedidosProntosNaoEntreguesPeriodo(30);

                Page.ClientScript.RegisterStartupScript(typeof(Painel), "alertaAdministrador",
                    "<Script>openWindow(400, 550, '../Utils/AlertaAdministrador.aspx?boletoVencendo=" + (vencimentoBoleto ? "true" : "false") +
                    "&pedidosProntosNaoEntregues=" + (pedidosProntosNaoEntregues ? "true" : "false") + "');</script>");
            }

            if (AvaliacaoAtendimentoDAO.Instance.PossuiAvaliacaoPendente())
            {
                Page.ClientScript.RegisterStartupScript(typeof(Painel), "alertaAvaliacao",
                    "openWindow(600, 800, '../Utils/AvaliacaoAtendimento.aspx');", true);
            }
        }
        protected void lnkNovaMensagem_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadMensagem.aspx?popup=" + Request["popup"]);
        }
        
        protected void lnkNovaMensagemParceiro_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadMensagemParceiro.aspx");
        }

        protected void lnkEnviadas_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstMensagensEnviadas.aspx");
        }
    }
}
