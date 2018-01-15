using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadVeiculos : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            dtvVeiculos.Register("~/Listas/LstVeiculos.aspx");
            odsVeiculos.Register();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["Placa"] != null)
                dtvVeiculos.ChangeMode(DetailsViewMode.Edit);

            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarVeiculo))
                Response.Redirect("~/WebGlass/Main.aspx");
        }
    
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Listas/LstVeiculos.aspx");
        }
    }
}
