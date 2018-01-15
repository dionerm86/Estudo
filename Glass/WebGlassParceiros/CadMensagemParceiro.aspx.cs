using System;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace Glass.UI.Web.WebGlassParceiros
{
    public partial class CadMensagemParceiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(WebGlassParceiros.CadMensagemParceiro));
    
            if (!IsPostBack && !String.IsNullOrEmpty(Request["idFuncDest"]))
            {
                var funcionarioFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();
                // Recupera os dados do funcionário
                var funcionario = funcionarioFluxo.ObtemFuncionario(Request["idFuncDest"].StrParaInt());

                // Se for resposta de mensagem, inclui destinatário e assunto
                ClientScript.RegisterStartupScript(typeof(string), "scr", "setDest(" + funcionario.IdFunc +
                    ", '" + BibliotecaTexto.GetFirstName(funcionario.Nome) + "');", true);
    
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
                    IdRemetente = (int)Glass.Data.Helper.UserInfo.GetUserInfo.IdCliente,
                    IsFunc = false
                };

                if (!string.IsNullOrEmpty(destinatarios))
                    // Insere os destinatários
                    msg.DestinatariosFuncionario.AddRange(destinatarios.Split(',').Where(f => !string.IsNullOrEmpty(f))
                        .Select(f =>
                            new Glass.Global.Negocios.Entidades.DestinatarioParceiroFuncionario
                            {
                                IdFunc = int.Parse(f)
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
