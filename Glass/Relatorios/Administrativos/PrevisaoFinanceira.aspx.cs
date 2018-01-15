using System;
using System.Web.UI.WebControls;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios.Administrativos
{
    public partial class PrevisaoFinanceira : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!PedidoConfig.LiberarPedido)
                {
                    chkPrevisaoPedido.Visible = false;
                    trPrevisaoPedidos.Visible = false;
                }
            }
    
            trPrevisaoCustoFixo.Visible = false;
    
            hdfData.Value = DateTime.Now.ToShortDateString();
    
            var receb = GetPrevisaoFinanceira(odsPrevFinancReceb.Select() as object[]);
            var pagar = GetPrevisaoFinanceira(odsPrevFinancPagar.Select() as object[]);
    
            lblVencidasMais90DiasReceb.Text = receb.VencidasMais90Dias.ToString("C");
            lblVencidas90DiasReceb.Text = receb.Vencidas90Dias.ToString("C");
            lblVencidas60DiasReceb.Text = receb.Vencidas60Dias.ToString("C");
            lblVencidas30DiasReceb.Text = receb.Vencidas30Dias.ToString("C");
            lblVencimentoHojeReceb.Text = receb.VencimentoHoje.ToString("C");
            lblVencer30DiasReceb.Text = receb.Vencer30Dias.ToString("C");
            lblVencer60DiasReceb.Text = receb.Vencer60Dias.ToString("C");
            lblVencer90DiasReceb.Text = receb.Vencer90Dias.ToString("C");
            lblVencerMais90DiasReceb.Text = receb.VencerMais90Dias.ToString("C");
    
            lblChequesVencidosMais90DiasReceb.Text = receb.ChequesVencidosMais90Dias.ToString("C");
            lblChequesVencidos90DiasReceb.Text = receb.ChequesVencidos90Dias.ToString("C");
            lblChequesVencidos60DiasReceb.Text = receb.ChequesVencidos60Dias.ToString("C");
            lblChequesVencidos30DiasReceb.Text = receb.ChequesVencidos30Dias.ToString("C");
            lblChequesVencimentoHojeReceb.Text = receb.ChequesVencimentoHoje.ToString("C");
            lblChequesVencer30DiasReceb.Text = receb.ChequesVencer30Dias.ToString("C");
            lblChequesVencer60DiasReceb.Text = receb.ChequesVencer60Dias.ToString("C");
            lblChequesVencer90DiasReceb.Text = receb.ChequesVencer90Dias.ToString("C");
            lblChequesVencerMais90DiasReceb.Text = receb.ChequesVencerMais90Dias.ToString("C");
    
            lblPedidosVencidosMais90Dias.Text = receb.PedidosVencidosMais90Dias.ToString("C");
            lblPedidosVencidos90Dias.Text = receb.PedidosVencidos90Dias.ToString("C");
            lblPedidosVencidos60Dias.Text = receb.PedidosVencidos60Dias.ToString("C");
            lblPedidosVencidos30Dias.Text = receb.PedidosVencidos30Dias.ToString("C");
            lblPedidosVencimentoHoje.Text = receb.PedidosVencimentoHoje.ToString("C");
            lblPedidosVencer30Dias.Text = receb.PedidosVencer30Dias.ToString("C");
            lblPedidosVencer60Dias.Text = receb.PedidosVencer60Dias.ToString("C");
            lblPedidosVencer90Dias.Text = receb.PedidosVencer90Dias.ToString("C");
            lblPedidosVencerMais90Dias.Text = receb.PedidosVencerMais90Dias.ToString("C");

            lblTotalVencMais90DiasReceb.Text = receb.TotalVencidasMais90Dias.ToString("C");
            lblTotalVenc90DiasReceb.Text = receb.TotalVencidas90Dias.ToString("C");
            lblTotalVenc60DiasReceb.Text = receb.TotalVencidas60Dias.ToString("C");
            lblTotalVenc30DiasReceb.Text = receb.TotalVencidas30Dias.ToString("C");
            lblTotalVencHojeReceb.Text = receb.TotalVencimentoHoje.ToString("C");
            lblTotalVencer30DiasReceb.Text = receb.TotalVencer30Dias.ToString("C");
            lblTotalVencer60DiasReceb.Text = receb.TotalVencer60Dias.ToString("C");
            lblTotalVencer90DiasReceb.Text = receb.TotalVencer90Dias.ToString("C");
            lblTotalVencerMais90DiasReceb.Text = receb.TotalVencerMais90Dias.ToString("C");
    
            lblVencidasMais90DiasPagar.Text = pagar.VencidasMais90Dias.ToString("C");
            lblVencidas90DiasPagar.Text = pagar.Vencidas90Dias.ToString("C");
            lblVencidas60DiasPagar.Text = pagar.Vencidas60Dias.ToString("C");
            lblVencidas30DiasPagar.Text = pagar.Vencidas30Dias.ToString("C");
            lblVencimentoHojePagar.Text = pagar.VencimentoHoje.ToString("C");
            lblVencer30DiasPagar.Text = pagar.Vencer30Dias.ToString("C");
            lblVencer60DiasPagar.Text = pagar.Vencer60Dias.ToString("C");
            lblVencer90DiasPagar.Text = pagar.Vencer90Dias.ToString("C");
            lblVencerMais90DiasPagar.Text = pagar.VencerMais90Dias.ToString("C");
    
            lblChequesVencidosMais90DiasPagar.Text = pagar.ChequesVencidosMais90Dias.ToString("C");
            lblChequesVencidos90DiasPagar.Text = pagar.ChequesVencidos90Dias.ToString("C");
            lblChequesVencidos60DiasPagar.Text = pagar.ChequesVencidos60Dias.ToString("C");
            lblChequesVencidos30DiasPagar.Text = pagar.ChequesVencidos30Dias.ToString("C");
            lblChequesVencimentoHojePagar.Text = pagar.ChequesVencimentoHoje.ToString("C");
            lblChequesVencer30DiasPagar.Text = pagar.ChequesVencer30Dias.ToString("C");
            lblChequesVencer60DiasPagar.Text = pagar.ChequesVencer60Dias.ToString("C");
            lblChequesVencer90DiasPagar.Text = pagar.ChequesVencer90Dias.ToString("C");
            lblChequesVencerMais90DiasPagar.Text = pagar.ChequesVencerMais90Dias.ToString("C");
    
            if (pagar.PrevisaoCustoFixoVencer30Dias.HasValue)
                lblPrevisaoCustoFixoVencer30DiasPagar.Text = pagar.PrevisaoCustoFixoVencer30Dias.Value.ToString("C");
            else
                lblPrevisaoCustoFixoVencer30DiasPagar.Text = "R$0,00";
    
            if (pagar.PrevisaoCustoFixoVencer60Dias.HasValue)
                lblPrevisaoCustoFixoVencer60DiasPagar.Text = pagar.PrevisaoCustoFixoVencer60Dias.Value.ToString("C");
            else
                lblPrevisaoCustoFixoVencer60DiasPagar.Text = "R$0,00";
    
            if (pagar.PrevisaoCustoFixoVencer90Dias.HasValue)
                lblPrevisaoCustoFixoVencer90DiasPagar.Text = pagar.PrevisaoCustoFixoVencer90Dias.Value.ToString("C");
            else
                lblPrevisaoCustoFixoVencer90DiasPagar.Text = "R$0,00";
    
            lblTotalVencMais90DiasPagar.Text = pagar.TotalVencidasMais90Dias.ToString("C");
            lblTotalVenc90DiasPagar.Text = pagar.TotalVencidas90Dias.ToString("C");
            lblTotalVenc60DiasPagar.Text = pagar.TotalVencidas60Dias.ToString("C");
            lblTotalVenc30DiasPagar.Text = pagar.TotalVencidas30Dias.ToString("C");
            lblTotalVencHojePagar.Text = pagar.TotalVencimentoHoje.ToString("C");
            lblTotalVencer30DiasPagar.Text = pagar.TotalVencer30Dias.ToString("C");
            lblTotalVencer60DiasPagar.Text = pagar.TotalVencer60Dias.ToString("C");
            lblTotalVencer90DiasPagar.Text = pagar.TotalVencer90Dias.ToString("C");
            lblTotalVencerMais90DiasPagar.Text = pagar.TotalVencerMais90Dias.ToString("C");
        }

        private Data.RelModel.PrevisaoFinanceira GetPrevisaoFinanceira(object[] dados)
        {
            Data.RelModel.PrevisaoFinanceira item = null;
            if (dados.Length > 0)
                item = dados[0] as Data.RelModel.PrevisaoFinanceira;
    
            if (item == null)
                item = new Data.RelModel.PrevisaoFinanceira();
    
            return item;
        }
    
        protected void PrevisaoCustoFixo_CheckChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                trPrevisaoCustoFixo.Visible = true;
            else
                trPrevisaoCustoFixo.Visible = false;
        }
    
        protected void PrevisaoPedido_CheckChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
                trPrevisaoPedidos.Visible = true;
            else
                trPrevisaoPedidos.Visible = false;
        }
    }
}
