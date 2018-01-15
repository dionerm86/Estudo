using System;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadCaixaGeral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ((TextBox)ctrlDataIni.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Text = DateTime.Now.ToString("dd/MM/yyyy");
    
                lblTituloContasContabeis.Text += " " + FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil;
                lblTituloContasNaoContabeis.Text += " " + FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil;
            }
    
            LoginUsuario login = UserInfo.GetUserInfo;

            var financGeral = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);

            // Verifica se o funcionário só pode pesquisar dia a dia no caixa geral, se for o caso, ao selecionar a data inicial 
            // replica a data selecionada para a data fim.
            if (!(Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento)))
            {
                ((ImageButton)ctrlDataFim.FindControl("imgData")).Visible = false;
                ((TextBox)ctrlDataFim.FindControl("txtHora")).Style.Add("display", "none");
                ((TextBox)ctrlDataFim.FindControl("txtData")).Style.Add("display", "none");
                lblPeriodo.Visible = false;
    
                ((TextBox)ctrlDataIni.FindControl("txtHora")).Style.Add("display", "none");
                ctrlDataIni.CallbackSelecionaData = "setDataFim(FindControl('ctrlDataIni_txtData', 'input'));";
            }
            
            // Verifica se a empresa trabalha com total do caixa geral
            if (!FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo || !financGeral)
            {
                lblTitleTotalDinheiro.Visible = false;
                lblTotalDinheiro.Visible = false;
    
                lblTitleTotalCheque.Visible = false;
                lblTitleTotalChequeTerc.Visible = false;
                lblTitleTotalChequeReapresentado.Visible = false;
                lblTitleTotalChequeDev.Visible = false;
    
                lblTotalCheque.Visible = false;
                lblTotalTercVenc.Visible = false;
                lblTotalChequeReapresentado.Visible = false;
                lblTotalChequeDev.Visible = false;
            }
    
            if (FinanceiroConfig.FinanceiroRec.ApenasFinancGeralAdminSelFuncCxGeral && !financGeral)
            {
                drpFuncionario.DataBind();
                drpFuncionario.SelectedValue = login.CodUser.ToString();
                drpFuncionario.Enabled = false;
            }
        }
    
        protected void grdCaixaGeral_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdCaixaGeral.Rows)
            {
                if (String.IsNullOrEmpty(((Label)row.Cells[0].FindControl("lblCodMov")).Text))
                {
                    row.ForeColor = System.Drawing.Color.Black;
                    row.Font.Bold = true;
                }
    
                if (((HiddenField)row.Cells[0].FindControl("hdfTipoMov")).Value == "2")
                    foreach (TableCell c in row.Cells)
                        c.ForeColor = System.Drawing.Color.Red;
            }
    
            if (grdCaixaGeral.Rows.Count > 0)
            {
                // Total geral de dinheiro, crédito, construcard, cheque e cheques de terceiro em aberto e vencidos
                lblTotalDinheiro.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfTotalDinheiro")).Value;
                lblTotalCheque.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfTotalCheque")).Value;
                lblTotalChequeDev.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfTotalChequeDev")).Value;
                lblTotalTercVenc.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfTotalTercVenc")).Value;
                lblTotalChequeReapresentado.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfTotalChequeReapres")).Value;
                
                // Saldo do período consultado
                lblSaldoDinheiro.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaldoDinheiro")).Value;
                lblSaldoCheque.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaldoCheque")).Value;
                lblSaldoCartao.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaldoCartao")).Value;
                lblSaldoConstrucard.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaldoConstrucard")).Value;
                lblSaldoPermuta.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaldoPermuta")).Value;

                // Total de saída do caixa
                lblSaidaDinheiro.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaidaDinheiro")).Value;
                lblSaidaCheque.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaidaCheque")).Value;
                lblSaidaCartao.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaidaCartao")).Value;
                lblSaidaConstrucard.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaidaConstrucard")).Value;
                lblSaidaPermuta.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfSaidaPermuta")).Value;

                // Total de entrada do caixa
                lblEntradaDinheiro.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfEntradaDinheiro")).Value;
                lblEntradaCheque.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfEntradaCheque")).Value;
                lblEntradaCartao.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfEntradaCartao")).Value;
                lblEntradaConstrucard.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfEntradaConstrucard")).Value;
                lblEntradaPermuta.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfEntradaPermuta")).Value;

                // Crédito gerado/recebido
                lblCreditoGerado.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfCreditoGerado")).Value;
                lblCreditoRecebido.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfCreditoRecebido")).Value;
    
                // Notas promissórias
                lblContasReceberGeradas.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfContasReceberGeradas")).Value;
    
                // Contas recebidas contábeis
                lblContasContabeis.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfContasRecebidasContabeis")).Value;
                
                // Contas recebidas não contábeis
                lblContasNaoContabeis.Text = ((HiddenField)grdCaixaGeral.Rows[0].Cells[0].FindControl("hdfContasRecebidasNaoContabeis")).Value;
            }
            else
            {
                string textoVazio = "R$ 0,00";
                lblTotalDinheiro.Text = textoVazio;
                lblTotalCheque.Text = textoVazio;
                lblTotalChequeDev.Text = textoVazio;
                lblTotalChequeReapresentado.Text = textoVazio;
                lblTotalTercVenc.Text = textoVazio;
                lblSaldoDinheiro.Text = textoVazio;
                lblSaldoCheque.Text = textoVazio;
                lblSaldoCartao.Text = textoVazio;
                lblSaldoConstrucard.Text = textoVazio;
                lblSaldoPermuta.Text = textoVazio;
                lblSaidaDinheiro.Text = textoVazio;
                lblSaidaCheque.Text = textoVazio;
                lblSaidaCartao.Text = textoVazio;
                lblSaidaConstrucard.Text = textoVazio;
                lblSaidaPermuta.Text = textoVazio;
                lblEntradaDinheiro.Text = textoVazio;
                lblEntradaCheque.Text = textoVazio;
                lblEntradaCartao.Text = textoVazio;
                lblEntradaConstrucard.Text = textoVazio;
                lblEntradaPermuta.Text = textoVazio;
                lblCreditoGerado.Text = textoVazio;
                lblCreditoRecebido.Text = textoVazio;
                lblContasReceberGeradas.Text = textoVazio;
                lblContasContabeis.Text = textoVazio;
                lblContasNaoContabeis.Text = textoVazio;
            }

            lblContasReceberGeradas.Visible = false;
            Label16.Visible = false;
        }
    
        protected void drpContabil_IndexChanged(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "0")
                tbCaixaGeral.FindControl("resumoCaixaGeral").Visible = false;
            else
                tbCaixaGeral.FindControl("resumoCaixaGeral").Visible = true;
        }
    
        protected string ExibirColunasContaRecebida()
        {
            return FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber ? "" : "display: none";
        }
    }
}
