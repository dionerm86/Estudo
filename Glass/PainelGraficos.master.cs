using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Glass.UI.Web
{
    public partial class PainelGraficos : System.Web.UI.MasterPage
    {
        public int TempoSegundosAtualizar { get; set; }

        public string ConteudoPainel { get; set; }

        public List<string> MensagensRodape { get; private set; }
    
        public PainelGraficos()
        {
            TempoSegundosAtualizar = 1800;
            ConteudoPainel = String.Empty;
            MensagensRodape = new List<string>();
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(PainelGraficos));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!(Page.Master.Master as Layout).IsPopup())
                Response.Redirect(Request.Url.ToString() + (Request.QueryString.Count > 0 ? "&" : "?") + "popup=true&exibirCabecalhoPopup=true", true);

            lblConteudoPainel.Text = ConteudoPainel;

            // Altera o tamanho do rodapé de acordo com a configuração por empresa.
            lblMsgRodape.Font.Size = 50;

            // Salva na mensagem do rodapé as mensagens informadas
            lblMsgRodape.Text = "";
            foreach (var s in MensagensRodape)
                lblMsgRodape.Text += s + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        }

        [Ajax.AjaxMethod()]
        public void GetUsuarioLogado()
        {
            var usuario = UserInfo.GetUserInfo;
            if (usuario == null || usuario.CodUser == null)
                return;
            UserInfo.SetActivity();
        }
    }
}