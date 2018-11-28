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
        //        // Impede o acesso não autorizado à esta tela
        //        if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario))
        //            Response.Redirect("../WebGlass/Main.aspx");
        //        // Somente o usuário Admin Sync pode editar seu próprio cadastro.
        //        else if (FuncionarioDAO.Instance.IsAdminSync(Request["idFunc"].StrParaUint()) &&
        //            !UserInfo.GetUserInfo.IsAdminSync)
        //            Response.Redirect("../WebGlass/Main.aspx");
        //    }
        //}
    }
}
