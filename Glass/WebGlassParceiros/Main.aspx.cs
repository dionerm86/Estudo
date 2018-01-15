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
             * O usu�rio informou que as mensagens enviadas n�o poderiam ficar sem refer�ncia de destinat�rio,
             * portanto, criamos esta configura��o para permitir ou n�o a exclus�o de mensagens. */
            grdMensagem0.Columns[1].Visible = Glass.Configuracoes.MensagemConfig.TelaMensagens.ExibirBotaoExcluirMensagem;
        }
    
        protected void lnkNovaMensagem_Click(object sender, EventArgs e)
        {
            Response.Redirect("CadMensagemParceiro.aspx");
        }
    }
}
