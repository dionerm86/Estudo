using System;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadFechamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(CadFechamento));

            if (!IsPostBack)
            {
                ddlLoja.DataBound += new EventHandler(ddlLoja_DataBound);
    
                txtDataIni.Text = DateTime.Now.ToString("dd/MM/yyyy");
    
                // Esconde campos de transferência para o caixa geral se não for permitido selecionar o valor a ser transferido
                if (!FinanceiroConfig.CaixaDiario.InformarValorTransf)
                {
                    txtValorTransf.Attributes.Add("style", "display: none");
                    txtValorTransfAtraso.Attributes.Add("style", "display: none");
                }
                else
                {
                    lblValorTransf.Attributes.Add("style", "display: none");
                    lblValorTransfAtraso.Attributes.Add("style", "display: none");
                }
            }
    
            LoginUsuario login = UserInfo.GetUserInfo;
    
            #region Filtro por funcionário
    
            // Exibe/esconde o filtro
            tituloFunc.Visible = FinanceiroConfig.TelaFechamentoCaixaDiario.FiltroFuncionarioCaixaDiario;
            filtroFunc.Visible = tituloFunc.Visible;
    
            if (!IsPostBack)
            {
                // Seleciona o funcionário
                if (drpFunc.Items.Count == 1) drpFunc.DataBind();
                drpFunc.SelectedValue = tituloFunc.Visible ? UserInfo.GetUserInfo.CodUser.ToString() : "0";
    
                // Habilita o filtro
                drpFunc.Enabled = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
                pesqFunc.Visible = tituloFunc.Visible && drpFunc.Enabled;
            }

            #endregion

            var financGeral = Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);

            if (!financGeral)
            {
                if (!FinanceiroConfig.TelaFechamentoCaixaDiario.PermitirCaixaAlterarDataConsulta)
                {
                    imgDataIni.Visible = false;
                    imgPesq.Visible = false;
                    txtDataIni.Enabled = false;
                }
    
                // O valor da drop loja fica igual à loja do caixa logado
                ddlLoja.SelectedValue = login.IdLoja.ToString();
            }
            else if (Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario)) // Se não tiver permissão de caixa diário
            {
                divFecharCaixa.Visible = false;
                ddlLoja.Enabled = true;
            }
        }
    
        protected void ddlLoja_DataBound(object sender, EventArgs e)
        {
            ExibirFechamentoCaixa(false);
        }
    
        protected void ddlLoja_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExibirFechamentoCaixa(true);
        }
    
        protected void odsFechamento_Selected(object sender, Colosoft.WebControls.VirtualObjectDataSourceStatusEventArgs e)
        {
            ExibirFechamentoCaixa(true);
        }
    
        protected void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            try
            {
                // Pega o id da loja que está sendo fechada o caixa
                uint idLoja = uint.Parse(ddlLoja.SelectedValue);

                /* Chamado 63322. */
                if (string.IsNullOrEmpty(txtValorTransf.Text))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o valor a ser transferido para o caixa geral.", Page);
                    return;
                }
    
                CaixaDiarioDAO.Instance.FechaCaixa(idLoja, decimal.Parse(txtValorTransf.Text), DateTime.Now, false);
    
                grdFechamento.DataBind();
    
                Glass.MensagemAlerta.ShowMsg("Transferência concluída.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
            }
        }
    
        protected void btnFecharCaixaAtraso_Click(object sender, EventArgs e)
        {
            try
            {
                // Pega o id da loja que está sendo fechada o caixa
                uint idLoja = Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue);

                /* Chamado 63322. */
                if (string.IsNullOrEmpty(txtValorTransf.Text))
                {
                    Glass.MensagemAlerta.ShowMsg("Informe o valor a ser transferido para o caixa geral.", Page);
                    return;
                }
    
                CaixaDiarioDAO.Instance.FechaCaixa(idLoja, decimal.Parse(txtValorTransfAtraso.Text), CaixaDiarioDAO.Instance.GetDataCaixaAberto(idLoja), true);
    
                // Esconde div para fechar caixa não fechado e mostra div para fechar caixa de hoje
                divFecharCaixa.Visible = true;
                divFecharCaixaAtraso.Visible = false;
    
                grdFechamento.DataBind();
    
                Glass.MensagemAlerta.ShowMsg("Transferência concluída.", Page);
            }
            catch (Exception ex)
            {
                Glass.MensagemAlerta.ErrorMsg(null, ex, Page);
            }
        }
    
        protected void grdFechamento_DataBound(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdFechamento.Rows)
            {
                if (((HiddenField)row.Cells[0].FindControl("hdfTipoMov")).Value == "2")
                    foreach (TableCell c in row.Cells)
                        c.ForeColor = System.Drawing.Color.Red;
            }
    
            if (!String.IsNullOrEmpty(ddlLoja.SelectedValue))
            {
                decimal saldo = CaixaDiarioDAO.Instance.GetSaldoByLoja(Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue));
                decimal saldoDiaAnterior = 0;
    
                if (saldo == 0 && !CaixaDiarioDAO.Instance.ExisteMovimentacao(UserInfo.GetUserInfo.IdLoja, DateTime.Now))
                {
                    saldo = CaixaDiarioDAO.Instance.GetSaldoDiaAnterior(UserInfo.GetUserInfo.IdLoja);
                    saldoDiaAnterior = saldo;
                }
    
                lblSaldoCaixa.Text = saldo.ToString("C");
                lblSaldoDinheiro.Text = (CaixaDiarioDAO.Instance.GetSaldoByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, 
                    Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue), 0, DateTime.Now, 1) + saldoDiaAnterior).ToString("C");
                txtValorTransf.Text = saldo.ToString();
                lblValorTransf.Text = saldo.ToString("C");
            }
        }
    
        private void ExibirFechamentoCaixa(bool executarPostBack)
        {
            LoginUsuario login = UserInfo.GetUserInfo;
    
            // Se o funcionário tiver acesso ao controle do caixa diário
            if (Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
            {
                if (String.IsNullOrEmpty(ddlLoja.SelectedValue) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                {
                    ddlLoja.DataBind();
                    ddlLoja.SelectedValue = login.IdLoja.ToString();
                }
    
                // Se o caixa do dia anterior não tiver sido fechado, mostra div para fechá-lo
                bool cxFechadoDiaAnterior = CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue));
                divFecharCaixa.Visible = cxFechadoDiaAnterior && !CaixaDiarioDAO.Instance.CaixaFechado(Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue));
                divFecharCaixaAtraso.Visible = !cxFechadoDiaAnterior;
    
                // Se o caixa não tiver sido fechado no dia anterior, carrega valores.
                if (!cxFechadoDiaAnterior && (executarPostBack || !IsPostBack))
                {
                    decimal saldoAnterior = CaixaDiarioDAO.Instance.GetSaldoDiaAnterior(Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue));
                    lblSaldoCaixaAtraso.Text = saldoAnterior.ToString("C");
                    lblSaldoDinheiroAtraso.Text = CaixaDiarioDAO.Instance.GetSaldoByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, 
                        Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue), 0, 
                        CaixaDiarioDAO.Instance.GetDataCaixaAberto(Glass.Conversoes.StrParaUint(ddlLoja.SelectedValue)), 1).ToString("C");
                    txtValorTransfAtraso.Text = saldoAnterior.ToString();
                    lblValorTransfAtraso.Text = saldoAnterior.ToString("C");
                }
            }
        }

        #region Metodos Ajax

        [Ajax.AjaxMethod()]
        public string ObtemSaldoAtual(string idLoja)
        {

            decimal saldo = 0;
            if (!string.IsNullOrEmpty(idLoja))
            {
                saldo = CaixaDiarioDAO.Instance.GetSaldoByLoja(Glass.Conversoes.StrParaUint(idLoja));
                decimal saldoDiaAnterior = 0;

                if (saldo == 0 && !CaixaDiarioDAO.Instance.ExisteMovimentacao(UserInfo.GetUserInfo.IdLoja, DateTime.Now))
                {
                    saldo = CaixaDiarioDAO.Instance.GetSaldoDiaAnterior(UserInfo.GetUserInfo.IdLoja);
                    saldoDiaAnterior = saldo;
                }

            }
            return saldo.ToString();
        }

        #endregion
    }
}
