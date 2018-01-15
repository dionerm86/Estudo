using System;
using Glass.Data.Helper;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadMensagem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadMensagem));
 
            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.PermitirEnvioMensagemInterna))
            {
                Response.Redirect("../WebGlass/Main.aspx");
                return;
            }
    
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
                destinatarios = destinatarios.Trim().TrimEnd(',');

                var msg = new Glass.Global.Negocios.Entidades.Mensagem
                {
                    Assunto = assunto,
                    Descricao = mensagem,
                    IdRemetente = (int)UserInfo.GetUserInfo.CodUser
                };

                if (destinatarios.Contains("-1"))
                {
                    var funcionarioFluxo = ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                    msg.Destinatarios.AddRange(funcionarioFluxo.ObtemFuncionariosAtivos()
                        .Select(f => 
                            new Glass.Global.Negocios.Entidades.Destinatario
                            {
                                IdFunc = f.Id
                            }));
                }
                else if (!string.IsNullOrEmpty(destinatarios))
                {
                    // Insere os destinatários
                    msg.Destinatarios.AddRange(destinatarios.Split(',').Where(f => !string.IsNullOrEmpty(f))
                        .Select(f =>
                            new Glass.Global.Negocios.Entidades.Destinatario
                            {
                                IdFunc = int.Parse(f)
                            }));
                }

                var resultado = ServiceLocator.Current
                    .GetInstance<Glass.Global.Negocios.IMensagemFluxo>()
                    .SalvarMensagem(msg);

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
