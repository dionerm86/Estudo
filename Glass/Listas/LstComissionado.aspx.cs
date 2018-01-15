using System;
using System.Web.UI;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstComissionado : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdComissionado.Register();
            odsComissionado.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarComissionado);
        }

        /// <summary>
        /// Verifica se pode editar o comissionado.
        /// </summary>
        /// <returns></returns>
        protected bool PodeEditar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarComissionado);
        }

        /// <summary>
        /// Verifica se pode apagar o comissionado.
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarComissionado);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Cadastros/CadComissionado.aspx");
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdComissionado.PageIndex = 0;
        }
    
        protected void drpSituacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdComissionado.PageIndex = 0;
        }
    }
}
