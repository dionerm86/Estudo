using System;
using System.Web.UI;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstTransportador : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdTransportador.Register();
            odsTransportador.Register();

            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarTransportadora))
            {
                lnkInserir.Visible = false;
                grdTransportador.Columns[0].Visible = false;
            }
        }

        /// <summary>
        /// Verifica se pode editar o transportador.
        /// </summary>
        /// <returns></returns>
        protected bool PodeEditar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarTransportadora);
        }

        /// <summary>
        /// Verifica se pode apagar o transportador.
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarTransportadora);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadTransportador.aspx");
        }
        
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdTransportador.PageIndex = 0;
        }
    }
}
