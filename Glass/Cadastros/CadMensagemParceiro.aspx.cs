using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMensagemParceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadMensagemParceiro));
    
            if (!IsPostBack && !String.IsNullOrEmpty(Request["idCliDest"]))
            {
                var clienteFluxo = ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IClienteFluxo>();
                var cliente = clienteFluxo.ObtemCliente(Request["idCliDest"].StrParaInt());

                // Se for resposta de mensagem, inclui destinatário e assunto
                ClientScript.RegisterStartupScript(typeof(string), "scr", "setDest(" + cliente.IdCli +
                    ", '" + BibliotecaTexto.GetFirstName(cliente.Nome) + "');", true);
    
                txtAssunto.Text = "RES: " + Request["assunto"];
    
                txtMensagem.Focus();
            }
        }
    
        [Ajax.AjaxMethod()]
        public string Enviar(string assunto, string destinatarios, string mensagem)
        {
            try
            {
                var msg = new Glass.Global.Negocios.Entidades.MensagemParceiro
                {
                    Assunto = assunto,
                    Descricao = mensagem,
                    IdRemetente = (int)Glass.Data.Helper.UserInfo.GetUserInfo.CodUser,
                    IsFunc = true
                };

                if (!string.IsNullOrEmpty(destinatarios))
                    // Insere os destinatários
                    msg.DestinatariosCliente.AddRange(destinatarios.Split(',').Where(f => !string.IsNullOrEmpty(f))
                        .Select(f =>
                            new Glass.Global.Negocios.Entidades.DestinatarioParceiroCliente
                            {
                                IdCli = int.Parse(f)
                            }));

                var resultado = ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                    .SalvarMensagemParceiro(msg);

                if (!resultado)
                    return "Erro\t" + resultado.Message.Format();

                else
                    return "ok\tMensagem enviada.";
            }
            catch (Exception ex)
            {
                return "Erro\t" + ex.Message;
            }
        }
    }
}
