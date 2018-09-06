using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Listas
{
    public partial class LstPedidosEspelho : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Geral.ControlePCP || !Config.PossuiPermissao(Config.FuncaoMenuPCP.VisualizarPedidosEmConferencia))
            {
                Response.Redirect("~/webglass/main.aspx");
                return;
            }
        }
    }
}