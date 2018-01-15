using System;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstRota : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            odsRota.Register();
            grdRota.Register();

            if (!Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarRota))
            {
                lnkInserir.Visible = false;
                grdRota.Columns[0].Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grdRota.Columns[7].HeaderText = "Núm. Mín. Dias Entrega " + 
                (RotaConfig.UsarDiasCorridosCalculoRota ? "(Corridos)" : "(Úteis)");
        }
    
        protected void lnkInserir_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Cadastros/CadRota.aspx");
        }
    }
}
