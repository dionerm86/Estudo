using Glass.Data.DAL;
using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Cadastros.Expedicao
{
    public partial class CadLeituraExpBalcaoMobile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.Expedicao.CadLeituraExpBalcao));

            if (!IsPostBack)
            {
                // Obtém os setores que o funcionário possui acesso
                var funcSetor = FuncionarioSetorDAO.Instance.GetSetores(UserInfo.GetUserInfo.CodUser);

                if (funcSetor.Count == 0)
                    Response.Redirect("../../WebGlass/Main.aspx");

                var setor = Data.Helper.Utils.ObtemSetor((uint)funcSetor[0].IdSetor);

                // Se não for exp. balcão, sai desta tela
                if (UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao ||
                    !FuncionarioSetorDAO.Instance.PossuiSetorEntregue(UserInfo.GetUserInfo.CodUser) ||
                    !Glass.Configuracoes.PCPConfig.UsarNovoControleExpBalcao)
                    Response.Redirect("../../WebGlass/Main.aspx");

                hdfTempoLogin.Value = setor.TempoLogin.ToString();

                UserInfo.SetActivity();

                hdfFunc.Value = UserInfo.GetUserInfo.CodUser.ToString();
            }
        }

        protected void lnkLgout_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            dtvExpBalcao.DataBind();
        }
    }
}