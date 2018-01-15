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
            TempoSegundosAtualizar = 240;
            ConteudoPainel = String.Empty;
            MensagensRodape = new List<string>();
        }
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Page.Master.Master as Layout).IsPopup())
                Response.Redirect(Request.Url.ToString() + (Request.QueryString.Count > 0 ? "&" : "?") + "popup=true&exibirCabecalhoPopup=true", true);

            lblConteudoPainel.Text = ConteudoPainel;

            // Altera o tamanho do rodap� de acordo com a configura��o por empresa.
            lblMsgRodape.Font.Size = 50;

            // Salva na mensagem do rodap� as mensagens informadas
            lblMsgRodape.Text = "";
            foreach (var s in MensagensRodape)
                lblMsgRodape.Text += s + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        }
    }
}