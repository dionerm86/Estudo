using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;
using System;

namespace Glass.UI.Web.Utils
{
    public partial class DetalheMensagem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.PermitirEnvioMensagemInterna))
                btnResponder.Visible = false;

            odsMensagem.Selected += odsMensagem_Selected;
        }

        private void odsMensagem_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var mensagemDetalhes = e.ReturnValue as Glass.Global.Negocios.Entidades.MensagemDetalhes;
                ServiceLocator.Current.GetInstance<Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens>()
                    .MarcarMensagemComoLida(mensagemDetalhes);
            }
        }
    
        protected void btnResponder_Click(object sender, EventArgs e)
        {
            var mensagem = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                .ObtemMensagem(Request["idMsg"].StrParaInt());

            Response.Redirect("../Cadastros/CadMensagem.aspx?popup=true&idFuncDest=" + mensagem.IdRemetente + "&assunto=" + mensagem.Assunto);
        }
    }
}
