using System;
using System.Web.UI;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstFuncionario : System.Web.UI.Page
    {
        #region Métodos Protegidos

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdFunc.Register();
            odsFunc.Register(); 
        }

        /// <summary>
        /// Verifica se pode apagar os registros de funcionário.
        /// </summary>
        /// <returns></returns>
        protected bool PodeEditar(bool adminSync)
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario) && (!adminSync || UserInfo.GetUserInfo.IsAdminSync);
        }

        /// <summary>
        /// Verifica se pode apagar os registros de funcionário.
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar(bool adminSync)
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario) && (!adminSync || UserInfo.GetUserInfo.IsAdminSync);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Impede o acesso não autorizado à esta tela
            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarFuncionario);
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadFuncionario.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdFunc.PageIndex = 0;
        }

        #endregion
    }
}
