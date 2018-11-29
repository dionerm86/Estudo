using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sync.Controls;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaVendasProd : System.Web.UI.Page
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
                    cbdSituacao.SelectedValue = ((int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado).ToString();
                    lblPeriodoSituacao.Text = "Período (Confirmação)";
                    chkAgruparLiberacao.Visible = false;
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
            grdVendasProd.Columns[1].Visible = chkAmbiente.Checked;
            grdVendasProd.Columns[2].Visible = chkAgruparLiberacao.Checked;
            grdVendasProd.Columns[5].Visible = chkAgruparCli.Checked;
    
            filtroNotaFiscal.Visible = chkAgruparLiberacao.Checked;
        }
    
        protected void lnkPesq_Click(object sender, EventArgs e)
        {
            grdVendasProd.PageIndex = 0;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdVendasProd.PageIndex = 0;
        }
    
        protected void cbdSubgrupo_DataBound(object sender, EventArgs e)
        {
            if (cbdGrupo.SelectedValue.IndexOf(',') > -1)
                cbdSubgrupo.Items.Clear();
        }
        protected void cbdTipoVenda_DataBound(object sender, EventArgs e)
        {
            // Define como filtro padrão pedidos À Vista e À Prazo
            foreach (ListItem li in ((CheckBoxListDropDown)sender).Items)
            {
                switch (Glass.Conversoes.StrParaUint(li.Value))
                {
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.AVista:
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo:
                    case (uint)Glass.Data.Model.Pedido.TipoVendaPedido.Obra:
                        li.Selected = true;
                        break;
                }
            }
        }
    
        protected void cbdSituacao_DataBound(object sender, EventArgs e)
        {
            cbdSituacao.SelectedValue = (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado + "," + (int)Glass.Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente;
        }
    }
}
