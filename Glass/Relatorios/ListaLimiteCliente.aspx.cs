using System;
using System.Collections;
using System.Web.UI;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Relatorios
{
    public partial class ListaLimiteCliente : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (!PedidoConfig.LiberarPedido)
            {
                Label3.Visible = false;
                txtIdLiberarPedido.Visible = false;
                imgPesq1.Visible = false;
                drpAgrupar.Items.Remove(drpAgrupar.Items.FindByValue("2"));
            }
    
            drpOrdenar.Items[3].Enabled = PedidoConfig.LiberarPedido;
        }
    
        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdLimiteCliente.PageIndex = 0;
        }
    
        protected void odsLimiteCliente_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            if (!(e.ReturnValue is IEnumerable))
                return;
    
            try
            {
                var retorno = (List<ContasReceber>)e.ReturnValue;
                lkbImprimir.Visible = retorno.Count > 0;
            }
            catch
            {
                var retorno = (ContasReceber[])e.ReturnValue;
                lkbImprimir.Visible = retorno.Length > 0;        
            }
        }
    
        protected void grdLimiteCliente_DataBound(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtNumCli.Text) && !String.IsNullOrEmpty(txtNomeCliente.Text))
            {
                bool buscarCheques = FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite && ("," + cbdBuscar.SelectedValue + ",").Contains(",5,");
    
                // Preenche o título dos totalizadores
                lblTituloChequesEmAberto.Text = buscarCheques ? "Total de cheques em aberto/não vencidos:" : String.Empty;
                lblTituloChequesDevolvidos.Text = buscarCheques ? "Total de cheques devolvidos:" : String.Empty;
                lblTituloChequesProtestados.Text = buscarCheques ? "Total de cheques protestados:" : String.Empty;
                lblTituloCheques.Text = buscarCheques ? "TOTAL GERAL DE CHEQUES:" : String.Empty;
    
                lblTituloContasReceber.Text = !PedidoConfig.LiberarPedido ? "TOTAL CONTAS A RECEBER:" : "TOTAL A RECEBER DE PEDIDOS LIBERADOS:";
                lblTituloPedidosEmAberto.Text = !PedidoConfig.LiberarPedido ? "TOTAL PEDIDOS EM ABERTO:" : "TOTAL DE PEDIDOS NÃO LIBERADOS:";
                
                lblTituloTotalDebitos.Text = "TOTAL GERAL DE DÉBITOS:";
                lblTituloCreditoCliente.Text = "Crédito do Cliente:";
                lblTituloLimiteUtilizado.Text = "Limite utilizado:";
                lblTituloLimiteConfigurado.Text = "Limite configurado:";
    
                // Calcula e preenche os totais
                decimal totalChequesEmAberto = buscarCheques ? ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ChequesEmAberto) : 0;
                decimal totalChequesDevolvidos = buscarCheques ? ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ChequesDevolvidos) : 0;
                decimal totalChequesProtestados = buscarCheques ? ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ChequesProtestados) : 0;
                decimal totalCheques = buscarCheques ? ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ChequesTotal) : 0;
    
                decimal totalContasRec = ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ContasAReceberTotal);
                decimal totalContasRecAntecip = ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.ContasAReceberAntecipadas);
                decimal totalPedidos = ContasReceberDAO.Instance.GetDebitosByTipo(Glass.Conversoes.StrParaUint(txtNumCli.Text), ContasReceberDAO.TipoDebito.PedidosEmAberto);
                decimal creditoCliente = ClienteDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(txtNumCli.Text));
                decimal limiteConfigurado = ClienteDAO.Instance.ObtemLimite(Glass.Conversoes.StrParaUint(txtNumCli.Text));
    
                lblTotalChequesEmAberto.Text = buscarCheques ? totalChequesEmAberto.ToString("C") : String.Empty;
                lblTotalChequesDevolvidos.Text = buscarCheques ? totalChequesDevolvidos.ToString("C") : String.Empty;
                lblTotalChequesProtestados.Text = buscarCheques ? totalChequesProtestados.ToString("C") : String.Empty;
                lblTotalCheques.Text = buscarCheques ? totalCheques.ToString("C") : String.Empty;
    
                lblTotalContasReceber.Text = totalContasRec.ToString("C") + (totalContasRecAntecip > 0 ? " (" + totalContasRecAntecip.ToString("C") + " antecipados)" : "");
                lblTotalPedidosEmAberto.Text = totalPedidos.ToString("C");
                lblTotalDebitos.Text = (totalCheques + totalContasRec + totalPedidos).ToString("C");
                lblCreditoCliente.Text = creditoCliente.ToString("C");
                lblLimiteUtilizado.Text = ((totalCheques + totalContasRec + totalPedidos) - creditoCliente).ToString("C");
                lblLimiteConfigurado.Text = limiteConfigurado.ToString("C");
            }
            else
            {
                lblTituloCheques.Text = String.Empty;
                lblTituloContasReceber.Text = String.Empty;
                lblTituloPedidosEmAberto.Text = String.Empty;
                lblTituloTotalDebitos.Text = String.Empty;
    
                lblTotalCheques.Text = String.Empty;
                lblTotalContasReceber.Text = String.Empty;
                lblTotalPedidosEmAberto.Text = String.Empty;
                lblTotalDebitos.Text = String.Empty;
    
                lblCreditoCliente.Text = 0.ToString("C");
                lblLimiteUtilizado.Text = 0.ToString("C");
                lblLimiteConfigurado.Text = 0.ToString("C");
            }
        }
    
        protected void cbdBuscar_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoAtivoLimite)
                    cbdBuscar.Items[1].Enabled = false;
    
                if (!FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite && !PedidoConfig.LiberarPedido)
                    cbdBuscar.Items[2].Enabled = false;
    
                if (!PedidoConfig.LiberarPedido)
                    cbdBuscar.Items[3].Enabled = false;
    
                if (!FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite)
                    cbdBuscar.Items[4].Enabled = false;
    
                // Para a RiberVidros não traz as contas a receber ao abrir a tela
                cbdBuscar.SelectedValue = "1,2,3,4,5";
    
                for (int i = cbdBuscar.Items.Count - 1; i >= 0; i--)
                    if (!cbdBuscar.Items[i].Enabled)
                        cbdBuscar.Items.RemoveAt(i);
            }
        }
    }
}
