using System;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class Main : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdMensagem0.Register();
            odsMensagemParceiro.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Configuracoes.MensagemConfig.TelaMensagens.ExibirMensagemWebGlassParceiros)
                Title = string.Empty;

            lnkNovaMensagem.Visible = Configuracoes.MensagemConfig.TelaMensagens.ExibirMensagemWebGlassParceiros;
            grdMensagem0.Visible = Configuracoes.MensagemConfig.TelaMensagens.ExibirMensagemWebGlassParceiros;

            /* Chamado 15992.
             * O usuário informou que as mensagens enviadas não poderiam ficar sem referência de destinatário,
             * portanto, criamos esta configuração para permitir ou não a exclusão de mensagens. */
            grdMensagem0.Columns[1].Visible = Glass.Configuracoes.MensagemConfig.TelaMensagens.ExibirBotaoExcluirMensagem;
        }
    
        protected void lnkNovaMensagem_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadMensagemParceiro.aspx");
        }
    }
}
