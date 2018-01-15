using System;
using Glass.Data.Helper;

namespace Glass.UI.Web.Listas
{
    public partial class LstVeiculos : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdVeiculos.Register();
            odsVeiculos.Register();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkInserir.Visible = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarVeiculo);
        }

        /// <summary>
        /// Verifica se pode editar o veículo.
        /// </summary>
        /// <returns></returns>
        protected bool PodeEditar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarVeiculo);
        }

        /// <summary>
        /// Verifica se pode apagar o veículo.
        /// </summary>
        /// <returns></returns>
        protected bool PodeApagar()
        {
            return Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarVeiculo);
        }

        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadVeiculos.aspx");
        }
    }
}
