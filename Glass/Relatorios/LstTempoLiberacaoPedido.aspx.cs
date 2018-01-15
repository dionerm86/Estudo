using Glass.Configuracoes;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Relatorios
{
    public partial class LstTempoLiberacaoPedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!PedidoConfig.LiberarPedido)
                Response.Redirect("../WebGlass/Main.aspx");

            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));

            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(-15).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");

            }
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdProducaoSituacao.PageIndex = 0;
        }

        
    }
}