using System;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class RelacaoVendas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
    
                if (!PedidoConfig.LiberarPedido)
                {
                    lblSituacao.Attributes.Add("style", "display: none");
                    drpSituacao.Attributes.Add("style", "display: none");
                    imgPesq1.Visible = false;
                    lblPeriodo.Text = "Período";
                }
    
                if (!PedidoConfig.Comissao.ComissaoPedido)
                    drpAgrupar.Items[2].Enabled = false;
            }
    
            grdRelacaoVendas.Columns[1].HeaderText = drpAgrupar.SelectedValue == "0" ? "Emissor" : "Vendedor";
        }
    }
}
