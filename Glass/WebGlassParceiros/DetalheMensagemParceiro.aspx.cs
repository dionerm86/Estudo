using System;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class DetalheMensagemParceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            odsMensagemParceiro.Selected += odsMensagem_Selected;
        }

        private void odsMensagem_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (e.Exception == null)
            {
                var mensagemDetalhes = e.ReturnValue as Glass.Global.Negocios.Entidades.MensagemDetalhes;
                ServiceLocator.Current.GetInstance<Glass.Global.UI.Web.Process.Mensagens.LeituraMensagens>()
                    .MarcarMensagemParceiroClienteComoLida(mensagemDetalhes);
            }
        }
    
        protected void btnResponder_Click(object sender, EventArgs e)
        {
            var mensagem = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                 .ObtemMensagemParceiro(Request["idMsg"].StrParaInt());

            Response.Redirect("CadMensagemParceiro.aspx?popup=true&idFuncDest=" + mensagem.IdRemetente + "&assunto=" + mensagem.Assunto);
        }
    }
}
