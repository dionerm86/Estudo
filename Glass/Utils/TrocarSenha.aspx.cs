using System;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.UI.Web.Utils
{
    public partial class TrocarSenha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoginUsuario login = UserInfo.GetUserInfo;
            if (login.IdCliente > 0 && Request["idCli"] != login.IdCliente.ToString())
                ClientScript.RegisterClientScriptBlock(typeof(string), "fechar", "closeWindow();", true);
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (Request["idFunc"] != null)
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>();

                // Recupera os dados do funcionário
                var funcionario = fluxo.ObtemFuncionario(Request["idFunc"].StrParaInt());

                if (funcionario == null)
                    Glass.MensagemAlerta.ShowMsg("Os dados do funcionário não foram encontrados", this);

                funcionario.Senha = txtNova.Text;

                // Salva os dados do funcionário
                var resultado = fluxo.SalvarFuncionario(funcionario);

                if (!resultado)
                    Glass.MensagemAlerta.ErrorMsg("Não foi possível trocar a senha do fucionário", resultado);
            }
            else if (Request["idCli"] != null)
                ClienteDAO.Instance.AlteraSenha(Glass.Conversoes.StrParaUint(Request["idCli"]), txtNova.Text);
            else if (Request["idEquipe"] != null)
            {
                Equipe equipe = EquipeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(Request["idEquipe"]));
                equipe.Senha = txtNova.Text;
                EquipeDAO.Instance.Update(equipe);
            }
    
            lblOk.Visible = true;
    
            ClientScript.RegisterClientScriptBlock(typeof(string), "refresh", "window.opener.redirectUrl(window.opener.location.href);", true);
        }
    }
}
