using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaProducaoProd : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!IsPostBack)
            {
                if (!PedidoConfig.LiberarPedido)
                {
                    lblSituacao.Style.Add("display", "none");
                    cbdSituacao.Style.Add("display", "none");
                    cbdSituacao.SelectedValue = "1";
                    lblPeriodoSituacao.Text = "Período (Confirmação)";
                }
    
                if (!PedidoConfig.Pedido_FastDelivery.FastDelivery)
                {
                    lblFastDelivery.Style.Add("display", "none");
                    drpFastDelivery.Style.Add("display", "none");
                }
    
                if (ProdutoConfig.TelaProdutosVendidosRelatorio.UsarFiltroDataPedidoComoPadrao)
                {
                    ((TextBox)ctrlDataIniPed.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("dd/MM/yyyy");
                    ((TextBox)ctrlDataFimPed.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    ((TextBox)ctrlDataIniSit.FindControl("txtData")).Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("dd/MM/yyyy");
                    ((TextBox)ctrlDataFimSit.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                }
            }
    
            grdVendasProd.Columns[0].Visible = chkAgruparPedido.Checked;
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdVendasProd.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendasProd.PageIndex = 0;
        }
    
        protected void drpSubgrupo_DataBound(object sender, EventArgs e)
        {
            if (cbdGrupo.SelectedValue.IndexOf(',') > -1)
            {
                drpSubgrupo.SelectedValue = "0";
                drpSubgrupo.Enabled = false;
            }
            else
                drpSubgrupo.Enabled = true;
        }
    
        protected void cbdSituacao_DataBound(object sender, EventArgs e)
        {
            cbdSituacao.SelectedValue = (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado + "," + (int)Glass.Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente;
        }
    }
}
