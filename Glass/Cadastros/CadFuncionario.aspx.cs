using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFuncionario : System.Web.UI.Page
    {
        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (Request["idFunc"] != null)
        //        dtvFuncionario.ChangeMode(DetailsViewMode.Edit);

        //    if (!IsPostBack)
        //    {
        //        // Impede o acesso n�o autorizado � esta tela
        //        if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario))
        //            Response.Redirect("../WebGlass/Main.aspx");
        //        // Somente o usu�rio Admin Sync pode editar seu pr�prio cadastro.
        //        else if (FuncionarioDAO.Instance.IsAdminSync(Request["idFunc"].StrParaUint()) &&
        //            !UserInfo.GetUserInfo.IsAdminSync)
        //            Response.Redirect("../WebGlass/Main.aspx");
        //    }
        //}
    }
}
