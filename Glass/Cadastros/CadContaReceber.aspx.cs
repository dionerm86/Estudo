using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.Helper;
using System.Drawing;
using Glass.Configuracoes;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadContaReceber : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /* Chamado 11247. */
            if (!IsPostBack)
            {
                if (FinanceiroConfig.FiltroContasVinculadasMarcadoPorPadrao &&
                    chkExibirContasVinculadas != null)
                    chkExibirContasVinculadas.Checked = true;

                if (drpFiltroContasAntecipadas != null)
                    drpFiltroContasAntecipadas.SelectedValue = "1";
            }

            if (!String.IsNullOrEmpty(Request["rel"]))
            {
                divReceber.Visible = false;
                grdConta.Columns[0].Visible = false;
                grdConta.Columns[1].Visible = false;
            }

            if (PedidoConfig.LiberarPedido)
                grdConta.Columns[5].Visible = true;

            uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

            CarregaParcelasRenegociar();

            if (!FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                dadosCnab.Style.Add("display", "none");
                grdConta.Columns[12].Visible = false;
                grdConta.Columns[13].Visible = false;
                grdConta.Columns[14].Visible = false;
            }

            if (FinanceiroConfig.FinanceiroRec.ImpedirRecebimentoPorLoja &&
                tipoFunc != (uint) Data.Helper.Utils.TipoFuncionario.Administrador)
            {
                drpLoja.SelectedValue = UserInfo.GetUserInfo.IdLoja.ToString();
                lblLoja.Style.Add("display", "none");
                drpLoja.Style.Add("display", "none");
            }

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                grdConta.Columns[11].Visible = false;
            }
            
            grdConta.Columns[12].Visible = drpArquivoRemessa.SelectedValue != "1" &&
                                           FinanceiroConfig.FinanceiroRec.ExibirCnab;

            hdfCxDiario.Value = Request["cxDiario"];

            Ajax.Utility.RegisterTypeForAjax(typeof (MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof (CadContaReceber));
        }

        [Ajax.AjaxMethod()]
        public string Receber(string idPedidoStr, string idContaR, string dataRecebido, string fPagtos, string valores,
            string contas, string tpCartoes, string tpBoleto, string txAntecip, string juros, string parcial, string gerarCredito, string creditoUtilizado,
            string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto, string descontarComissao, string depositoNaoIdentificado, 
            string cartaoNaoIdentificado, string numAutCartao)
        {
            return WebGlass.Business.ContasReceber.Fluxo.Receber.Ajax.ReceberConta(idPedidoStr, idContaR, dataRecebido, fPagtos,
                valores, contas, tpCartoes, tpBoleto, txAntecip, juros, parcial, gerarCredito, creditoUtilizado, cxDiario,
                numAutConstrucard, parcCredito, chequesPagto, descontarComissao, depositoNaoIdentificado, cartaoNaoIdentificado, numAutCartao);
        }

        [Ajax.AjaxMethod()]
        public string Renegociar(string idPedido, string idContaR, string idFormaPagto, string numParc, string parcelas, string multa)
        {
            return WebGlass.Business.ContasReceber.Fluxo.Receber.Ajax.Renegociar(idPedido, idContaR, idFormaPagto, numParc,
                parcelas, multa);
        }

        [Ajax.AjaxMethod()]
        public string TemCnabGerado(string idContaR)
        {
            return ContasReceberDAO.Instance.TemCnabGerado(Glass.Conversoes.StrParaUint(idContaR)).ToString();
        }

        [Ajax.AjaxMethod()]
        public void MarcarJuridicoCartorio(string idContaR, bool juridico)
        {
            ContasReceberDAO.Instance.MarcarJuridico(idContaR.StrParaInt(), juridico);
        }

        protected void imgPesq_Click(object sender, ImageClickEventArgs e)
        {
            grdConta.PageIndex = 0;
            grdConta.DataBind();
        }

        protected void drpParcCredito_Load(object sender, EventArgs e)
        {
            ((DropDownList)sender).Visible = FinanceiroConfig.Cartao.PedidoJurosCartao;
        }

        protected void ctrlParcelas_Load(object sender, EventArgs e)
        {
            Glass.UI.Web.Controls.ctrlParcelas ctrlParcelas = (Glass.UI.Web.Controls.ctrlParcelas)sender;

            ctrlParcelas.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas.CampoParcelasVisiveis = drpNumParc;
            ctrlParcelas.CampoValorTotal = lblValor;
        }

        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;

            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = lblValor;
            ctrlFormaPagto1.CampoClienteID = hdfIdCliente;
        }

        private void CarregaParcelasRenegociar()
        {
            drpNumParc.Items.Clear();
            for (int i = 0; i < FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar; i++)
                drpNumParc.Items.Add(new ListItem((i + 1).ToString()));

            ctrlParcelas.NumParcelas = FinanceiroConfig.FormaPagamento.NumeroParcelasRenegociar;
        }

        protected void grdConta_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            ContasReceber conta = e.Row.DataItem as ContasReceber;

            if (conta.IsParcelaCartao || conta.IdContaRCartao > 0)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Green;

            else if (conta.IdArquivoRemessa.GetValueOrDefault(0) > 0)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = conta.Protestado ? Color.FromArgb(225, 200, 0) : Color.Blue;

            else if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico && conta.Juridico)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.FromArgb(225, 200, 0);

            else if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && conta.LiberacaoNaoPossuiNotaFiscalGerada)
                foreach (TableCell c in e.Row.Cells)
                    c.ForeColor = Color.Red;
        }

        protected void cbdAgrupar_Load(object sender, EventArgs e)
        {
            if (PedidoConfig.Comissao.ComissaoPedido && cbdAgrupar.Items.FindByValue("4") == null)
                cbdAgrupar.Items.Insert(1, new ListItem("Comissionado", "4"));

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && cbdAgrupar.Items.FindByValue("5") == null)
            {
                cbdAgrupar.Width = new Unit("200px");
                cbdAgrupar.Items.Add(new ListItem(String.Format("Tipo de Conta ({0} / {1} / {2} / Reposição)",
                    FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil,
                    FinanceiroConfig.ContasPagarReceber.DescricaoContaCupomFiscal), "5"));
            }
        }

        protected void drpRota_Load(object sender, EventArgs e)
        {
            if (!RotaDAO.Instance.ExisteRota())
            {
                ((DropDownList)sender).Style.Add("display", "none");
                lblRota.Style.Add("display", "none");
                imgPesqRota.Style.Add("display", "none");
            }
        }

        protected void drpFiltroContasAntecipadas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpFiltroContasAntecipadas.SelectedValue != "0")
                divReceber.Visible = false;
            else
                divReceber.Visible = true;
        }

        protected void drpArquivoRemessa_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                drpArquivoRemessa.SelectedValue = "2";
        }

        /// <summary>
        /// Esconde o drop de tipo entrega caso a empresa trabalhe com liberação de pedido
        /// o filtro não funciona nesse caso
        /// </summary>
        protected void drpTipoEntrega_Load(object sender, EventArgs e)
        {
            var visibilidade = !PedidoConfig.LiberarPedido;
            drpTipoEntrega.Visible = visibilidade;
            Label14.Visible = visibilidade;
        }

        /// <summary>
        /// Atualiza os pagamentos feitos com o cappta tef
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="admCodes"></param>
        /// <param name="customerReceipt"></param>
        /// <param name="merchantReceipt"></param>
        /// <param name="formasPagto"></param>
        [Ajax.AjaxMethod]
        public void AtualizaPagamentos(string id, string checkoutGuid, string admCodes, string customerReceipt, string merchantReceipt, string formasPagto)
        {
            TransacaoCapptaTefDAO.Instance.AtualizaPagamentosCappta(Data.Helper.UtilsFinanceiro.TipoReceb.ContaReceber, id.StrParaInt(),
                checkoutGuid, admCodes, customerReceipt, merchantReceipt, formasPagto);
        }

        /// <summary>
        /// Cancela o pagto que foi pago com TEF porem deu algum erro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motivo"></param>
        [Ajax.AjaxMethod]
        public void CancelarContaReceberErroTef(string id, string motivo)
        {
            ContasReceberDAO.Instance.CancelarConta(id.StrParaUint(), "Falha no recebimento TEF. Motivo: " + motivo, DateTime.Now, true, false);
        }
    }
}