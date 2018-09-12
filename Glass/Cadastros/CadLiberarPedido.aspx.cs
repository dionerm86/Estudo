using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Linq;
using Glass.Configuracoes;
using GDA;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadLiberarPedido : System.Web.UI.Page
    {
        private float _totM = 0;
        private decimal _total = 0;
        private float _qtde = 0;
        private int _qtdeAmbiente = 0;
        private decimal _icms = 0;
        private uint _idPedido = 0;
        private bool _isMaoDeObra = false;
        private List<uint> _prodPedPesoM2 = new List<uint>();
    
        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadLiberarPedido));
    
            hdfCxDiario.Value = Request["cxDiario"];
    
            if (String.IsNullOrEmpty(hdfBuscarIdsPedidos.Value))
            {
                tbPagto.Visible = false;
                btnConfirmar.Visible = false;
                chkTaxaPrazo.Visible = false;
            }
            
            hdfDataBase.Value = DateTime.Now.ToString("dd/MM/yyyy");
            legenda.Visible = false; 
    
            creditoClientePrazo.Visible = true;
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "pedidosAbertos", "getPedidosAbertos();\n");
    
            if (hdfPedidosAbertos.Value != String.Empty)
            {
                var idsPedidos = hdfPedidosAbertos.Value.Split(',');

                var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idsPedidos.First().StrParaUint());
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);
                legenda.Visible = (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && Liberacao.DadosLiberacao.LiberarPedidoProdutos;

                foreach (var id in idsPedidos)
                {                   
                    var botao = "getBotao(" + id + ")";
                    Page.ClientScript.RegisterStartupScript(GetType(), "exibirProdutos_" + id, "exibirProdutos(" + botao + ", " + id + ");\n", true);
                }
            }
    
            if (hdfIdsPedidosRem.Value != String.Empty)
            {
                lblPedidosRem.Text = "Pedidos Removidos: " + hdfIdsPedidosRem.Value.TrimEnd(',');
                imbLimparRemovidos.Visible = true;
            }

            if (!string.IsNullOrWhiteSpace(hdfBuscarIdsPedidos.Value))
            {
                var idsLojas = PedidoDAO.Instance.ObtemIdsLojas(hdfBuscarIdsPedidos.Value);
                var lojas = LojaDAO.Instance.GetByString(idsLojas);

                // Esconde os dados sobre o ICMS/IPI se a empresa não calcular
                grdPedido.Columns[8].Visible = lojas.Any(f=> f.CalcularIcmsPedido);
                lblTotalIcms.Visible = lojas.Any(f => f.CalcularIcmsPedido);
                lblTotalIcms.Visible = lojas.Any(f => f.CalcularIpiPedido);
            }            
    
            grdPedido.Columns[14].Visible = PedidoConfig.Pedido_FastDelivery.FastDelivery;
            grdPedido.Columns[15].Visible = PCPConfig.ControlarProducao;
    
            hdfDataTela.Value = DateTime.Now.ToString();
    
            if (!IsPostBack)
            {
                if (Glass.Configuracoes.Geral.NaoVendeVidro())
                {
                    lblSituacaoProd.Style.Add("display", "none");
                    drpSituacaoProd.Style.Add("display", "none");
                }
    
                cbdTipoPedido.SelectedValue = Liberacao.TelaLiberacao.TiposPedidosSelecionadosPadrao;
            }

            if (Request["cxDiario"] == "1")
            {
                lnkReceberSinal.OnClientClick = "openWindow(700, 850, 'CadReceberSinal.aspx?popup=1&cxDiario=1'); return false;";
                lnkPagtoAntecip.OnClientClick = "openWindow(700, 850, 'CadReceberSinal.aspx?antecipado=1&popup=1&cxDiario=1'); return false;";
            }
    
            var usarControleOC = OrdemCargaConfig.UsarControleOrdemCarga;
            lblOC.Visible = usarControleOC;
            txtNumOC.Visible = usarControleOC;
            imbAddOC.Visible = usarControleOC;
        }
    
        protected void btnBuscarPedidos_Click(object sender, EventArgs e)
        {
            grdPedido.DataBind();
            AtualizaFormaPagto();
        }
    
        #region Confirma à Vista
    
        [Ajax.AjaxMethod()]
        public string ConfirmarAVista(string idCliente, string idsPedido, string idsProdutosPedido, string idsProdutosProducao, string qtdeProdutosLiberar, string fPagtos, string tpCartoes,
            string totalASerPagoStr, string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string gerarCredito, string utilizarCredito, string creditoUtilizado,
            string numAutConstrucard, string cxDiario, string parcCredito, string descontarComissao, string chequesPagto, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr,
            string acrescimoStr, string valorUtilizadoObraStr, string numAutCartao, string usarCappta)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.Confirmar.Ajax.ConfirmarAVista(idCliente, idsPedido, idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar, fPagtos, tpCartoes,
                totalASerPagoStr, valores, contas, depositoNaoIdentificado, cartaoNaoIdentificado, gerarCredito, utilizarCredito, creditoUtilizado, numAutConstrucard, cxDiario, parcCredito,
                descontarComissao, chequesPagto, tipoDescontoStr, descontoStr, tipoAcrescimoStr, acrescimoStr, valorUtilizadoObraStr, numAutCartao, usarCappta);
        }
    
        #endregion
    
        #region Confirma à Prazo
    
        [Ajax.AjaxMethod()]
        public string ConfirmarAPrazo(string idCliente, string idsPedido, string idsProdutosPedido, 
            string idsProdutosProducao, string qtdeProdutosLiberar, string totalASerPagoStr, string numParcelasStr, string diasParcelasStr, 
            string idParcelaStr, string valoresParcelasStr, string receberEntradaStr, string fPagtos, string tpCartoes, string valores, 
            string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string utilizarCredito, string creditoUtilizado, string numAutConstrucard, string cxDiario, string parcCredito, 
            string descontarComissao, string tipoDescontoStr, string descontoStr, string tipoAcrescimoStr, 
            string acrescimoStr, string formaPagtoPrazoStr, string valorUtilizadoObraStr, string chequesPagto, string numAutCartao, string idsOc)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.Confirmar.Ajax.ConfirmarAPrazo(idCliente, idsPedido,
                idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar, totalASerPagoStr, numParcelasStr, diasParcelasStr,
                idParcelaStr, valoresParcelasStr, receberEntradaStr, fPagtos, tpCartoes, valores, contas, depositoNaoIdentificado, cartaoNaoIdentificado, utilizarCredito,
                creditoUtilizado, numAutConstrucard, cxDiario, parcCredito, descontarComissao, tipoDescontoStr,
                descontoStr, tipoAcrescimoStr, acrescimoStr, formaPagtoPrazoStr, valorUtilizadoObraStr, chequesPagto, numAutCartao, idsOc);
        }
    
        #endregion
    
        #region Confirma Garantia/Reposição
    
        [Ajax.AjaxMethod()]
        public string ConfirmarGarantiaReposicao(string idCliente, string idsPedido, string idsProdutosPedido, 
            string idsProdutosProducao, string qtdeProdutosLiberar)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.Confirmar.Ajax.ConfirmarGarantiaReposicao(idCliente,
                idsPedido, idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar);
        }
    
        #endregion
    
        #region Confirma Pedido de Funcionário
    
        [Ajax.AjaxMethod()]
        public string ConfirmarPedidoFuncionario(string idCliente, string idsPedido, string idsProdutosPedido,
            string idsProdutosProducao, string qtdeProdutosLiberar)
        {
            return WebGlass.Business.LiberarPedido.Fluxo.Confirmar.Ajax.ConfirmarPedidoFuncionario(idCliente,
                idsPedido, idsProdutosPedido, idsProdutosProducao, qtdeProdutosLiberar);
        }
    
        #endregion
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod]
        public string IsPedidosAlterados(string idsPedidos, string idsSinais, string idsPagtoAntecip, string dataTela)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.IsPedidosAlterados(idsPedidos, idsSinais, idsPagtoAntecip, dataTela);
        }
    
        [Ajax.AjaxMethod]
        public string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni, 
            string dataFim, string situacaoProd, string tiposPedidos, string idLoja)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.GetPedidosByCliente(idCliente, nomeCliente,
                idsPedidosRem, dataIni, dataFim, situacaoProd, tiposPedidos, idLoja);
        }
    
        [Ajax.AjaxMethod]
        public string ValidaPedido(string idPedidoStr, string tipoVendaStr, string idFormaPagtoStr, string cxDiario, string idsPedidoStr, string idsOcStr)
        {
            return WebGlass.Business.Pedido.Fluxo.BuscarEValidar.Ajax.ValidaPedido(idPedidoStr, tipoVendaStr, idFormaPagtoStr, cxDiario, idsPedidoStr, idsOcStr);
        }

        [Ajax.AjaxMethod]
        public string VerificarPedidosMesmaLoja(string idsPedidosStr)
        {            
            if (PedidoDAO.Instance.PedidosLojasDiferentes(idsPedidosStr))
            {                
                if(!FinanceiroConfig.DadosLiberacao.PermitirLiberacaoPedidosLojasDiferentes)
                    return "false|Não é possivel fazer a liberação de pedidos de lojas diferentes";
            }
            
            return "true|ok";
        }

        /// <summary>
        /// Recupera os ids dos pedidos de uma OC
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetIdsPedidosByOCForLiberacao(string idOC)
        {
            return WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Ajax.GetIdsPedidosByOCForLiberacao(idOC);
        }

        /// <summary>
        /// Recupera o tipo do cartão informado (Débito ou Crédito)
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public int ObterTipoCartao(int idTipoCartao)
        {
            return (int)TipoCartaoCreditoDAO.Instance.ObterTipoCartao(null, idTipoCartao);
        }

        /// <summary>
        /// Atualiza os pagamentos feitos com o cappta tef
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="checkoutGuid"></param>
        /// <param name="admCodes"></param>
        /// <param name="customerReceipt"></param>
        /// <param name="merchantReceipt"></param>
        /// <param name="formasPagto"></param>
        [Ajax.AjaxMethod]
        public void AtualizaPagamentos(string idLiberarPedido, string checkoutGuid, string admCodes, string customerReceipt, string merchantReceipt, string formasPagto)
        {
            /*TransacaoCapptaTefDAO.Instance.AtualizaPagamentosCappta(UtilsFinanceiro.TipoReceb.LiberacaoAVista , idLiberarPedido.StrParaInt(),
                checkoutGuid, admCodes, customerReceipt, merchantReceipt, formasPagto);*/
        }

        /// <summary>
        /// Cancela a liberação que foi paga com TEF porem deu algum erro
        /// </summary>
        /// <param name="idLiberarPedido"></param>   
        /// <param name="motivo"></param>
        [Ajax.AjaxMethod]
        public void CancelarLiberacaoErroTef(string idLiberarPedido, string motivo)
        {
            LiberarPedidoDAO.Instance.CancelarLiberacao(idLiberarPedido.StrParaUint(), "Falha no recebimento TEF. Motivo: " + motivo, DateTime.Now, true, false);
        }

        /// <summary>
        /// Emitir NFC-e de liberações pagas com TEF
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string EmitirNFCe(string idLiberarPedido)
        {
            if (!FiscalConfig.UtilizaNFCe)
                return "";

            var idsPedidos = LiberarPedidoDAO.Instance.IdsPedidos(null, idLiberarPedido);
            var idLoja = LiberarPedidoDAO.Instance.ObtemIdLoja(idLiberarPedido.StrParaUint());
            var idCli = LiberarPedidoDAO.Instance.GetIdCliente(idLiberarPedido.StrParaUint());
            var percReducaoNfe = ClienteDAO.Instance.GetPercReducaoNFe(idCli);
            var percReducaoNfeRevenda = ClienteDAO.Instance.GetPercReducaoNFeRevenda(idCli);

            var idNf = NotaFiscalDAO.Instance.GerarNf(idsPedidos, idLiberarPedido, null, idLoja, percReducaoNfe, percReducaoNfeRevenda, null, idCli, false, null, false, true, true);

            var retEmissao = NotaFiscalDAO.Instance.EmitirNf(idNf, false, false);

            if (retEmissao != "Autorizado o uso da NF-e")
                throw new Exception(retEmissao);

            return idNf.ToString();
        }

        #endregion
    
        #region Parcelas e Formas de Pagto
    
        protected void ctrlParcelas1_Load(object sender, EventArgs e)
        {
            ctrlParcelas1.CampoValorTotal = hdfValorASerPagoPrazo;
            ctrlParcelas1.CampoCalcularParcelas = hdfCalcularParcelas;
            ctrlParcelas1.CampoDataBase = hdfDataBase;
            ctrlParcelas1.CampoValorEntrada = ctrlFormaPagto2.FindControl("txtValorPago");
            ctrlParcelas1.CampoValorDescontoAtual = txtDesconto;
            ctrlParcelas1.CampoTipoDescontoAtual = drpTipoDesconto;
            ctrlParcelas1.CampoValorAcrescimoAtual = txtAcrescimo;
            ctrlParcelas1.CampoTipoAcrescimoAtual = drpTipoAcrescimo;
            ctrlParcelas1.CampoValorObra = hdfValorObra;
            ctrlParcelas1.CampoExibirParcelas = hdfExibirParcelas;
            
            // Define que serão somados 30 dias em caso da parcela estar vazia para resolver chamado 7508
            ctrlParcelas1.DiasSomarDataVazia = 30;
        }
    
        protected void ctrlFormasPagto_Load(object sender, EventArgs e)
        {
            Controls.ctrlFormaPagto fp = (Controls.ctrlFormaPagto)sender;
            fp.CampoCredito = hdfValorCredito;
            fp.CampoValorConta = fp.ID == "ctrlFormaPagto1" ? hdfTotalASerPago : 
                fp.ID == "ctrlFormaPagto2" ? hdfValorASerPagoPrazo : 
                ctrlParcelas1.FindControl("txtValorParcelas");
            
            fp.CampoValorDesconto = txtDesconto;
            fp.CampoTipoDesconto = drpTipoDesconto;
            fp.CampoValorAcrescimo = txtAcrescimo;
            fp.CampoTipoAcrescimo = drpTipoAcrescimo;
            fp.CampoClienteID = hdfIdCliente;
            fp.CampoValorObra = hdfValorObra;

            fp.UsarCreditoMarcado = Liberacao.DadosLiberacao.UsarCreditoMarcadoTelaLiberacaoPedido;
        }
    
        protected void drpTipoPagto_Load(object sender, EventArgs e)
        {
            float valorPagar = float.TryParse(hdfTotalASerPago.Value, out valorPagar) ? valorPagar : 0;
            uint idCliente = Conversoes.StrParaUint(hdfIdCliente.Value);
    
            // Se houver separação entre valores fiscais e reais só permite liberação à prazo
            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                drpTipoPagto.Items[0].Enabled = valorPagar <= 0;
                drpTipoPagto.Items[1].Enabled = valorPagar > 0;
                chkReceberEntrada.Visible = false;

                ctrlFormaPagto1.ExibirGerarCredito = valorPagar < 0;
                ctrlFormaPagto2.ExibirGerarCredito = valorPagar < 0;
            }
            else if (idCliente > 0)
            {
                chkReceberEntrada.Visible = true;
                bool isAPrazo = valorPagar > 0;

                var possuiParcelaAVista = ParcelasDAO.Instance.VerificarPossuiParcelaAVista(null, hdfIdsPedido.Value != null ? hdfIdsPedido.Value.Split(',').Select(f => f.StrParaInt()) : null);

                if (isAPrazo && (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido || PedidoConfig.Desconto.DescontoPedidoApenasAVista ||
                    Liberacao.DadosLiberacao.UsarMenorPrazoLiberarPedido || possuiParcelaAVista))
                {
                    List<int?> tipoVenda = GetTipoVendaPedidos();
    
                    // Só permite liberar à prazo se houver pedidos à prazo e não houver pedidos à vista
                    isAPrazo = (tipoVenda.Contains((int)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo) || Liberacao.TelaLiberacao.CobrarPedidoReposicao) &&
                        !tipoVenda.Contains((int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista);
                }
    
                // Verifica as parcelas do cliente se o cliente só puder pagar nas parcelas que ele tem atribuídas
                isAPrazo = isAPrazo && ParcelasDAO.Instance.GetCountByCliente(idCliente, ParcelasDAO.TipoConsulta.Prazo) > 0;
    
                drpTipoPagto.Items[1].Enabled = isAPrazo;

                if (isAPrazo)
                {
                    uint? tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(idCliente);

                    if (tipoPagto > 0)
                    {
                        var parcela = ParcelasDAO.Instance.GetElementByPrimaryKey(tipoPagto.Value);

                        if (tipoPagto != null && parcela != null && parcela.NumParcelas > 0)
                            drpTipoPagto.SelectedIndex = 1;

                        ctrlParcelasSelecionar1.ParcelaPadrao = tipoPagto;
                    }
                }
            }
    
            if (valorPagar < 0)
                Page.ClientScript.RegisterStartupScript(GetType(), "esconderDescAcresc", "escondeDescontoAcrescimo();\n", true);
        }
    
        #endregion
    
        #region Métodos usados na página
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        private string GetColorName(System.Drawing.Color cor)
        {
            if (cor.Name == "0")
                return "White";
    
            return cor.IsNamedColor ? cor.Name : "#" + cor.Name.Substring(2);
        }
    
        #endregion
    
        protected void grdProdutosPedido_DataBound(object sender, EventArgs e)
        {
            var grid = (GridView)sender;
    
            var exibirQtde = true;
            for (var i = 0; i < grid.Rows.Count; i++)
                if (!String.IsNullOrEmpty(((HiddenField)grid.Rows[i].FindControl("hdfIdProdPedProducao")).Value))
                {
                    exibirQtde = false;
                    break;
                }

            var ignorar = true;

            if (hdfPedidosAbertos.Value != String.Empty)
            {
                var idsPedidos = hdfPedidosAbertos.Value.Split(',');

                var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idsPedidos.First().StrParaUint());
                ignorar = LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);
            }

            var idCliente = Glass.Conversoes.StrParaUint(((HiddenField)grid.Parent.Parent.FindControl("hdfIdCliente")).Value);

            exibirQtde = exibirQtde || (!Liberacao.DadosLiberacao.LiberarProdutosProntos || ignorar) ||
                (RotaClienteDAO.Instance.IsClienteAssociado(idCliente) && Liberacao.DadosLiberacao.LiberarClienteRota);

            if (!string.IsNullOrWhiteSpace(hdfBuscarIdsPedidos.Value))
            {
                var idsLojas = PedidoDAO.Instance.ObtemIdsLojas(hdfBuscarIdsPedidos.Value);
                var lojas = LojaDAO.Instance.GetByString(idsLojas);

                grid.Columns[6].Visible = lojas.Any(f => f.CalcularIcmsPedido);
            }

            grid.Columns[8].Visible = exibirQtde;
            grid.Columns[9].Visible = exibirQtde;
            grid.Columns[10].Visible = !exibirQtde;

            IniciaTreeView(grid);
        }

        protected void grdProdutosPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var idPedido = e.Row.DataItem != null ? Glass.Conversoes.StrParaUint(DataBinder.Eval(e.Row.DataItem, "IdPedido").ToString()) : 0;
    
            if (!Liberacao.DadosLiberacao.LiberarPedidoProdutos && (idPedido > 0 && !PedidoDAO.Instance.IsPedidoAtrasado(null, idPedido, true)))
                return;

            var idLoja = PedidoDAO.Instance.ObtemIdLoja(null, idPedido);
            var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);

            if (PCPConfig.UsarConferenciaFluxo && (Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && e.Row.FindControl("hdfCorLinha") != null)
            {
                var nomeCorLinha = ((HiddenField)e.Row.FindControl("hdfCorLinha")).Value;
                var corLinha = Color.FromName(nomeCorLinha);

                if (corLinha != Color.Blue && corLinha != Color.Black && corLinha != Color.Green)
                {
                    var chkSelProdPed = (CheckBox)e.Row.FindControl("chkSelProdPed");
                    chkSelProdPed.Checked = false;
                    chkSelProdPed.Enabled = false;
    
                    e.Row.Cells[8].Text = "0";
    
                    var txtQtde = (TextBox)e.Row.FindControl("txtQtde");
                    txtQtde.Text = "0";
                    txtQtde.Enabled = false;
                }
    
                for (int i = 1; i < e.Row.Cells.Count; i++)
                    e.Row.Cells[i].ForeColor = corLinha;
            }
    
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (idPedido != _idPedido)
                {
                    _idPedido = idPedido;
                    _isMaoDeObra = PedidoDAO.Instance.IsMaoDeObra(null, idPedido);
                }

               
                var ignorar = LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(null, idLoja);

                _icms += decimal.Parse(DataBinder.Eval(e.Row.DataItem, "ValorIcms").ToString());
                _qtde += !String.IsNullOrEmpty(e.Row.Cells[8].Text) ? float.Parse(e.Row.Cells[8].Text) : float.Parse(DataBinder.Eval(e.Row.DataItem, "QtdeDisponivelLiberacao").ToString());
                _qtdeAmbiente += _isMaoDeObra && (!Liberacao.DadosLiberacao.LiberarProdutosProntos || ignorar) ? Glass.Conversoes.StrParaInt(DataBinder.Eval(e.Row.DataItem, "QtdeAmbiente").ToString()) : 0;
                _total += decimal.Parse(DataBinder.Eval(e.Row.DataItem, "TotalCalc").ToString());
                _totM += float.Parse(DataBinder.Eval(e.Row.DataItem, "TotM2Liberacao").ToString());
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[5].Text = _totM.ToString("0.##");
                e.Row.Cells[6].Text = _icms.ToString("C");
                e.Row.Cells[7].Text = _total.ToString("C");
                e.Row.Cells[8].Text = _qtde.ToString() + (_qtdeAmbiente > 0 ? " x " + _qtdeAmbiente + " p.v." : "");
    
                _totM = 0;
                _total = 0;
                _qtde = 0;
                _qtdeAmbiente = 0;
                _icms = 0;
                _idPedido = 0;
            }
        }
    
        protected void grdPedido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;
    
            _totM = 0;
            _total = 0;
            _qtde = 0;
            _qtdeAmbiente = 0;
            _icms = 0;
            _idPedido = 0;
    
            if (grdPedido.Columns[14].Visible)
            {
                var p = e.Row.DataItem as Glass.Data.Model.Pedido;
                ((Label)e.Row.FindControl("Label9")).ForeColor = p.SituacaoProducao == (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pronto ? Color.Blue :
                    p.SituacaoProducao == (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Entregue ? Color.Green : Color.Red;
            }
    
            var grid = e.Row.FindControl("grdProdutosPedido") as GridView;
    
            if (grid != null && grid.Visible)
                grid.DataBind();
        }
    
        protected void grdPedido_DataBound(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(hdfBuscarIdsPedidos.Value))
            {
                var idsLojas = PedidoDAO.Instance.ObtemIdsLojas(hdfBuscarIdsPedidos.Value);
                var lojas = LojaDAO.Instance.GetByString(idsLojas);

                grdPedido.Columns[8].Visible = lojas.Any(f => f.CalcularIcmsPedido);
            }

            hdfBloqueioTipoVenda.Value = string.Empty;
            hdfBloqueioIdFormaPagto.Value = string.Empty;

            if (grdPedido.Rows.Count > 0)
            {
                // Verifica se todos os pedidos são do mesmo cliente
                var idCliente = Glass.Conversoes.StrParaUint(((HiddenField)grdPedido.Rows[0].FindControl("hdfIdCliente")).Value);
                
                for (var i = 1; i < grdPedido.Rows.Count; i++)
                {
                    var idClienteNovo = Glass.Conversoes.StrParaUint(((HiddenField)grdPedido.Rows[i].FindControl("hdfIdCliente")).Value);
                    if (idCliente != idClienteNovo)
                    {
                        lblMensagem.Text = "Apenas pedidos do mesmo cliente podem ser liberados.";
                        mensagemErro.Visible = true;
                        tbPagto.Visible = false;
                        btnConfirmar.Visible = false;
                        chkTaxaPrazo.Visible = false;
                        return;
                    }
                }

                var possuiParcelaAVista = ParcelasDAO.Instance.VerificarPossuiParcelaAVista(null, hdfBuscarIdsPedidos.Value != null ? hdfBuscarIdsPedidos.Value.Split(',').Select(f => f.StrParaInt()) : null);

                if (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido || possuiParcelaAVista)
                    hdfBloqueioTipoVenda.Value = ((HiddenField)grdPedido.Rows[0].FindControl("hdfTipoVenda")).Value;

                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                    hdfBloqueioIdFormaPagto.Value = ((HiddenField)grdPedido.Rows[0].FindControl("hdfIdFormaPagto")).Value;

                mensagemErro.Visible = false;
                CalcularPrecos();
            }
            else
            {
                tbPagto.Visible = false;
                btnConfirmar.Visible = false;
                chkTaxaPrazo.Visible = false;
            }
        }
    
        private void CalcularPrecos()
        {
            decimal totalASerPago = 0;
            decimal totalASerPagoPrazo = 0;
            decimal totalIcms = 0;
            decimal totalIpi = 0;
            decimal totalDesconto = 0;
            decimal totalPedidos = 0;
    
            var idPedidoValido = new List<string>();
    
            var idsPedido = String.Empty;
            var idsProdutosPedido = String.Empty;
            var idsProdutoPedidoProducao = String.Empty;
            var qtdeProdutosLiberar = String.Empty;
            var idsOc = hdfIdsOc.Value;

            decimal valorObra = 0;
            decimal valorJaPago = 0;
    
            // Caso tenha algum pedido nesta liberação com desconto de 100%, é necessário habilitar os campos para confirmar a liberação
            var possuiPedidoDescontoCemPorCento = false;
    
            // Se a empresa trabalha com crédito
            uint idCliente;
            if (txtNumCli.Text != "")
                idCliente = Glass.Conversoes.StrParaUint(txtNumCli.Text);
            else
                idCliente = Glass.Conversoes.StrParaUint(((HiddenField)grdPedido.Rows[0].FindControl("hdfIdCliente")).Value);
    
            List<int?> tipoVendaPedidos = GetTipoVendaPedidos();
    
            #region Busca pedidos marcados para liberação
    
            List<string> lstPedidos = new List<string>(hdfBuscarIdsPedidos.Value.Split(','));
    
            hdfLibParc.Value = "false";

            foreach (GridViewRow r in grdPedido.Rows)
            {
                decimal totalPedido = 0;
                uint idPedido = Glass.Conversoes.StrParaUint(((HiddenField)r.FindControl("hdfIdPedido")).Value);
                Glass.Data.Model.Pedido pedido = PedidoDAO.Instance.GetElementForLiberacao(idPedido);

                GridView grdProdutos = (GridView)r.FindControl("grdProdutosPedido");
                IniciaTreeView(grdProdutos);

                if (OrdemCargaConfig.UsarOrdemCargaParcial)
                    if (grdProdutos.HeaderRow != null && ((CheckBox)grdProdutos.HeaderRow.FindControl("chkTodos")) != null)
                        ((CheckBox)grdProdutos.HeaderRow.FindControl("chkTodos")).Enabled = false;

                // Se o pedido for garantia ou reposição, não cobra
                if (pedido.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Garantia || (pedido.TipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição &&
                !Liberacao.TelaLiberacao.CobrarPedidoReposicao))
                {
                    foreach (GridViewRow r1 in grdProdutos.Rows)
                    {
                        if (((CheckBox)r1.FindControl("chkSelProdPed")).Checked)
                        {
                            if (!idPedidoValido.Contains(idPedido.ToString()))
                                idPedidoValido.Add(idPedido.ToString());

                            string id = ((HiddenField)r1.FindControl("hdfIdProdPed")).Value;
                            string idProdPedProducao = ((HiddenField)r1.FindControl("hdfIdProdPedProducao")).Value;
                            bool liberarProntos = grdProdutos.Columns[10].Visible;

                            var chkSelProdPedCtrl = ((CheckBox)r1.FindControl("chkSelProdPed"));
                            var qtdeCtrl = ((TextBox)r1.FindControl("txtQtde"));

                            string qtdeMaxString = ((Label)r1.FindControl("lblQtdeDisp")).Text;
                            string qtdeString = qtdeCtrl.Text;

                            float qtde = liberarProntos && !String.IsNullOrEmpty(idProdPedProducao) ? 1 :
                                !String.IsNullOrEmpty(qtdeString) ? float.Parse(qtdeString) : 0;
    
                            float qtdeMax = liberarProntos && !String.IsNullOrEmpty(idProdPedProducao) ? 1 :
                                !String.IsNullOrEmpty(qtdeMaxString) ? float.Parse(qtdeMaxString) : 0;

                            if (OrdemCargaConfig.UsarOrdemCargaParcial)
                            {
                                qtdeCtrl.Enabled = false;
                                chkSelProdPedCtrl.Enabled = false;

                                if (pedido.OrdemCargaParcial)
                                {
                                    #region Verifica se o subgrupo permite produto de revenda em um pedido de venda

                                    var subgrupoPermiteItemRevendaNaVenda = false;
                                    var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(null, id.StrParaUint());

                                    if (idProd > 0)
                                    {
                                        var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);

                                        if (idSubgrupoProd > 0)
                                            subgrupoPermiteItemRevendaNaVenda = SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda((int)idSubgrupoProd);
                                    }

                                    #endregion

                                    /* Chamado 61312.
                                     * Caso o subgrupo permita produto de revenda em um pedido de venda, não é necessário obter a quantidade
                                     * parcial para a liberação, pois todas as peças devem ser liberadas através da primeira liberação. */
                                    qtde = subgrupoPermiteItemRevendaNaVenda ? qtde : ItemCarregamentoDAO.Instance.ObterQtdeLiberarParcial(null, id.StrParaUint(), idsOc);
                                    qtdeCtrl.Text = qtde.ToString();
                                }
                            }

                            if (qtde > qtdeMax)
                            {
                                qtde = qtdeMax;
                                qtdeCtrl.Text = qtdeMax.ToString();
                            }
                            else if (qtde < qtdeMax || qtdeMax == 0)
                                hdfLibParc.Value = "true";
    
                            idsProdutosPedido += id + ";";
                            qtdeProdutosLiberar += qtde + ";";
                            idsProdutoPedidoProducao += idProdPedProducao + ";";
                        }
                        else
                            hdfLibParc.Value = "true";
                    }
    
                    continue;
                }
    
                totalIcms += pedido.ValorIcms;
                totalIpi += pedido.ValorIpi;
    
                if (!lstPedidos.Contains(idPedido.ToString()))
                    continue;

                var existeEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido);
                var pedidoEspelho = existeEspelho ? PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(idPedido) : new PedidoEspelho();

                var valorRecEntrada = pedido.IdSinal > 0 ? pedido.ValorEntrada : 0;
                var valorRecPagtoAntecip = pedido.IdPagamentoAntecipado > 0 || pedido.IdObra > 0 ? pedido.ValorPagamentoAntecipado : 0;

                foreach (GridViewRow r1 in grdProdutos.Rows)
                {
                    if (((CheckBox)r1.FindControl("chkSelProdPed")).Checked)
                    {
                        if (!idPedidoValido.Contains(idPedido.ToString()))
                            idPedidoValido.Add(idPedido.ToString());

                        string id = ((HiddenField)r1.FindControl("hdfIdProdPed")).Value;
                        string idProdPedProducao = ((HiddenField)r1.FindControl("hdfIdProdPedProducao")).Value;
                        bool liberarProntos = grdProdutos.Columns[10].Visible;

                        var chkSelProdPedCtrl = ((CheckBox)r1.FindControl("chkSelProdPed"));
                        var qtdeCtrl = ((TextBox)r1.FindControl("txtQtde"));

                        string totalString = ((Label)r1.FindControl("lblTotal")).Text;
                        string qtdeMaxString = ((Label)r1.FindControl("lblQtdeDisp")).Text;
                        string qtdeString = qtdeCtrl.Text;

                        float qtde = liberarProntos && !String.IsNullOrEmpty(idProdPedProducao) ? 1 :
                            !String.IsNullOrEmpty(qtdeString) ? float.Parse(qtdeString) : 0;

                        float qtdeMax = liberarProntos && !String.IsNullOrEmpty(idProdPedProducao) ? 1 :
                            !String.IsNullOrEmpty(qtdeMaxString) ? float.Parse(qtdeMaxString) : 0;

                        decimal total = !String.IsNullOrEmpty(totalString) ? decimal.Parse(totalString.Trim(' ', '(', ')').Replace("R$", "").Replace(" ", "").Replace(".", "")) : 0;
                        if (totalString.IndexOf("(") > -1)
                            total *= -1;

                        if (OrdemCargaConfig.UsarOrdemCargaParcial)
                        {
                            qtdeCtrl.Enabled = false;
                            chkSelProdPedCtrl.Enabled = false;

                            if (pedido.OrdemCargaParcial)
                            {
                                #region Verifica se o subgrupo permite produto de revenda em um pedido de venda

                                var subgrupoPermiteItemRevendaNaVenda = false;
                                var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(null, id.StrParaUint());

                                if (idProd > 0)
                                {
                                    var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);

                                    if (idSubgrupoProd > 0)
                                        subgrupoPermiteItemRevendaNaVenda = SubgrupoProdDAO.Instance.ObtemPermitirItemRevendaNaVenda((int)idSubgrupoProd);
                                }

                                #endregion

                                /* Chamado 61312.
                                 * Caso o subgrupo permita produto de revenda em um pedido de venda, não é necessário obter a quantidade
                                 * parcial para a liberação, pois todas as peças devem ser liberadas através da primeira liberação. */
                                qtde = subgrupoPermiteItemRevendaNaVenda ? qtde : ItemCarregamentoDAO.Instance.ObterQtdeLiberarParcial(null, id.StrParaUint(), idsOc);
                                qtdeCtrl.Text = qtde.ToString();
                            }
                        }

                        if (qtde > qtdeMax)
                        {
                            qtde = qtdeMax;
                            qtdeCtrl.Text = qtdeMax.ToString();
                        }
                        else if (qtde < qtdeMax || qtdeMax == 0)
                            hdfLibParc.Value = "true";

                        idsProdutosPedido += id + ";";
                        qtdeProdutosLiberar += qtde + ";";
                        idsProdutoPedidoProducao += idProdPedProducao + ";";

                        // O valor total do pedido deve ser recuperado por aqui somente se a empresa permitir a liberação parcial dos pedidos,
                        // caso contrário, o total a ser liberado é recuperado da model de pedido.
                        if (qtde > 0)
                            totalPedido += total / (decimal)qtdeMax * (decimal)qtde;
                    }
                    else
                        hdfLibParc.Value = "true";
                }

                /* Chamado 54882. */
                totalPedido += existeEspelho ? pedidoEspelho.ValorEntrega : pedido.ValorEntrega;

                // Recupera a variável que será usada para cálculo do percentual calculado para o pedido
                // Usado para liberação parcial - aplica o desconto e subtrai parte do valor 
                // da entrada relativo ao percentual do pedido que está sendo liberado
                var dividir = (!existeEspelho || pedido.DescontoTotal == 0 || PedidoConfig.RatearDescontoProdutos) ? 0 : pedidoEspelho.TotalSemDesconto;

                // Define o tipo de desconto: se o valor calculado for do pedido espelho, usa o desconto do pedido espelho também
                pedido.DescontoTotalPcp = PCPConfig.UsarConferenciaFluxo && dividir > 0;

                // Caso não deva ser utilizado o total do pedido PCP e não exista conferência para este pedido, utilizad o total sem desconto
                // do pedido original
                dividir = (pedido.DescontoTotalPcp || PedidoConfig.RatearDescontoProdutos) ? dividir : !existeEspelho ? pedido.TotalSemDesconto : pedidoEspelho.TotalSemDesconto;

                // Remove o fast delivery do total calculado do pedido, aplica o desconto logo abaixo e depois aplica o fast delivery novamente
                // Faz o mesmo com o dividir
                if (pedido.FastDelivery && pedido.DescontoTotal > 0)
                {
                    totalPedido = totalPedido / (1 + ((decimal)pedido.TaxaFastDelivery / 100));
                    dividir = (existeEspelho ? pedidoEspelho.Total : pedido.Total) /
                        (1 + ((decimal)pedido.TaxaFastDelivery / 100)) + (existeEspelho ? pedidoEspelho.DescontoTotal : pedido.DescontoTotal);
                }

                var percentual = ((!PedidoConfig.RatearDescontoProdutos ? totalPedido - pedido.DescontoTotal : totalPedido) +
                    (!PedidoConfig.RatearDescontoProdutos ? pedido.DescontoTotal : 0)) / (dividir > 0 ? dividir : 1);


                var descontoReais = pedido.DescontoTotal * percentual;

                // Se o desconto dado no pedido for de 100%, a propriedade DescontoTotal retorna 0, é necessário fazer
                // este procedimento para que o desconto seja aplicado na liberação, até que seja feita uma forma que retorne o valor
                // correto do desconto em casos de desconto de 100%. 
                if (descontoReais == 0 && (pedido.TotalParaLiberacao == 0 || pedido.Desconto == 100) && pedido.TipoDesconto == 1)
                {
                    descontoReais = totalPedido;
                    possuiPedidoDescontoCemPorCento = true;
                }

                totalPedido = totalPedido - descontoReais;


                // Aplica o fast delivery novamente
                if (pedido.FastDelivery && pedido.DescontoTotal > 0)
                    /* Chamados 44552, 46882, 50020 e 50287. */
                    totalPedido = totalPedido * (1 + ((decimal)pedido.TaxaFastDelivery / 100));

                totalPedido -= (valorRecEntrada + valorRecPagtoAntecip) * (!PedidoConfig.RatearDescontoProdutos ? percentual : 1);                
                valorJaPago += valorRecEntrada + valorRecPagtoAntecip;

                if (chkTaxaPrazo.Checked)
                    totalPedido = totalPedido * (decimal)(1 + (pedido.TaxaPrazo / 100));
                
                totalASerPagoPrazo += totalPedido;
                totalASerPago += totalPedido;
                totalDesconto += pedido.DescontoTotal;
                totalPedidos += pedido.TotalParaLiberacao;
            }

            idsPedido = string.Join(",", idPedidoValido.ToArray());
            idsProdutosPedido = idsProdutosPedido.TrimEnd(';');
            idsProdutoPedidoProducao = string.IsNullOrEmpty(idsProdutoPedidoProducao) ? idsProdutoPedidoProducao : idsProdutoPedidoProducao.Remove(idsProdutoPedidoProducao.Length - 1, 1);
            qtdeProdutosLiberar = qtdeProdutosLiberar.TrimEnd(';');
    
            // Se for pedido de liberação ou de garantia, permite liberar
            bool isGarantiaReposicao = !String.IsNullOrEmpty(idsPedido) && (PedidoDAO.Instance.IsPedidoGarantia(null, idsPedido) || 
                PedidoDAO.Instance.IsPedidoReposicao(null, idsPedido)) && totalPedidos == 0;
    
            hdfIsGarantiaReposicao.Value = isGarantiaReposicao.ToString().ToLower();
    
            // Verifica se é pedido de funcionário
            bool isPedidoFuncionario = !String.IsNullOrEmpty(idsPedido) && PedidoDAO.Instance.IsPedidoFuncionario(idsPedido);
            hdfIsPedidoFuncionario.Value = isPedidoFuncionario.ToString().ToLower();

            var possuiParcelaAVista = ParcelasDAO.Instance.VerificarPossuiParcelaAVista(null, idsPedido != null ? idsPedido.Split(',').Select(f => f.StrParaInt()) : null);
            bool exibirMensagemErro = false;
            if (!isGarantiaReposicao && (String.IsNullOrEmpty(idsPedido) ||
                (Liberacao.DadosLiberacao.LiberarPedidoProdutos && String.IsNullOrEmpty(idsProdutosPedido))))
            {
                lblMensagem.Text = "Nenhum pedido selecionado.";
                exibirMensagemErro = true;
            }
            else if (!isGarantiaReposicao && (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido || possuiParcelaAVista) && tipoVendaPedidos.Count > 1)
            {
                lblMensagem.Text = "Para liberar esses pedidos eles devem ser do mesmo tipo de venda.";
                exibirMensagemErro = true;
            }

            // Busca as observações do cliente
            AtualizaObsCliente();
    
            if (exibirMensagemErro)
            {
                mensagemErro.Visible = true;
                tbPagto.Visible = false;
                btnConfirmar.Visible = false;
                chkTaxaPrazo.Visible = false;
                return;
            }
            else if (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido || possuiParcelaAVista)
                drpTipoPagto_Load(null, EventArgs.Empty);
    
            #endregion
    
            #region Crédito cliente
    
            if (!isGarantiaReposicao)
            {    
                hdfValorCredito.Value = ClienteDAO.Instance.GetCredito(idCliente).ToString().Replace(',', '.');
            }
    
            #endregion
    
            // Exibe o valor total do ICMS e IPI
            if (!isGarantiaReposicao)
            {
                lblTotalIcms.Text = "Total de ICMS ST: " + totalIcms.ToString("C");
                lblTotalIpi.Text = "Total de IPI: " + totalIpi.ToString("C");
            }
    
            // Guarda o id do cliente no HiddenField
            hdfIdCliente.Value = idCliente.ToString();
            drpTipoPagto_Load(null, EventArgs.Empty);
    
            // Guarda o id dos pedidos que serão liberados
            hdfIdsPedido.Value = idsPedido;
            hdfIdsProdutosPedido.Value = idsProdutosPedido;
            hdfIdsProdutoPedidoProducao.Value = idsProdutoPedidoProducao;
            hdfQtdeProdutosLiberar.Value = qtdeProdutosLiberar;
    
            // Arredonda os valores para 2 casas decimais
            totalASerPago = Math.Round(totalASerPago, 2);
            totalASerPagoPrazo = Math.Round(totalASerPagoPrazo, 2);
            valorJaPago = Math.Round(valorJaPago, 2);
    
            // Guarda o valor que deverá ser pago
            hdfTotalASerPago.Value = totalASerPago.ToString();
            hdfValorASerPagoPrazo.Value = totalASerPagoPrazo.ToString();
    
            // Salva o valor utilizado das obras, deve arrendodar para no mínimo para 4 casas decimais, para não acontecer de calcular
            // um valor decimal mundo pequeno e o javascript interpretar como notação científica, causando erros na liberação
            hdfValorObra.Value = Math.Round(valorObra, 4).ToString();
    
            tbPagto.Visible = ((totalASerPagoPrazo + valorJaPago) > 0 || (totalPedidos - totalDesconto == 0)) && !String.IsNullOrEmpty(idsProdutosPedido);
            btnConfirmar.Visible = tbPagto.Visible || isGarantiaReposicao || possuiPedidoDescontoCemPorCento;
            chkTaxaPrazo.Visible = false;
    
            // Define se a opção de desconto ficará escondida
            if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Data.Helper.Utils.TipoFuncionario.Administrador &&
                !String.IsNullOrEmpty(hdfIdCliente.Value) && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(Glass.Conversoes.StrParaUint(hdfIdCliente.Value), 0, null, 0, null))
            {
                lblDesconto.Attributes.Add("style", "display: none");
                drpTipoDesconto.Attributes.Add("style", "display: none");
                txtDesconto.Attributes.Add("style", "display: none");
                txtDesconto.Text = String.Empty;
            }
    
            // Esconde os campos de acréscimo
            if (!FinanceiroConfig.DadosLiberacao.ExibirAcrescimoNaLiberacao)
            {
                lblAcrescimo.Attributes.Add("style", "display: none");
                drpTipoAcrescimo.Attributes.Add("style", "display: none");
                txtAcrescimo.Attributes.Add("style", "display: none");
                txtAcrescimo.Text = String.Empty;
            }
    
            if (!isGarantiaReposicao)
            {
                var script = "try {\n";
                if (Liberacao.DadosLiberacao.LiberarPedidoProdutos)
                {
                    foreach (var idPedido in idsPedido.Split(','))
                    {
                        var botao = "getBotao(" + idPedido + ")";
                        script += "exibirProdutos(" + botao + ", " + idPedido + ");\n";
                    }
                }
    
                script += ctrlFormaPagto1.ClientID + ".AdicionarIDs('" + idsPedido + "');\n";
                script += ctrlFormaPagto2.ClientID + ".AdicionarIDs('" + idsPedido + "');\n";
    
                script += "} catch (err) { }\n";
                Page.ClientScript.RegisterStartupScript(GetType(), "abrirProdutos", script, true);
                drpTipoPagto_Load(null, EventArgs.Empty);
            }
        }
    
        protected void imbLimparRemovidos_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsPedidos.Value += "," + hdfIdsPedidosRem.Value.Trim(',');
            hdfIdsPedidosRem.Value = String.Empty;
            lblPedidosRem.Text = "";
            imbLimparRemovidos.Visible = false;
    
            grdPedido.DataBind();
        }
    
        protected void ctrlParcelasSelecionar1_Load(object sender, EventArgs e)
        {
            var parc = (Glass.UI.Web.Controls.ctrlParcelasSelecionar)sender;
            parc.ControleParcelas = ctrlParcelas1;
            parc.CampoClienteID = hdfIdCliente;
            parc.CampoPedidosIDs = hdfIdsPedido;
        }
    
        protected List<int?> GetTipoVendaPedidos()
        {
            var retorno = new List<int?>();
    
            foreach (GridViewRow r in grdPedido.Rows)
            {
                var tipoVendaStr = ((HiddenField)r.Cells[0].FindControl("hdfTipoVenda")).Value;
                var idPedidoStr = ((HiddenField)r.Cells[0].FindControl("hdfIdPedido")).Value;
                int? tipoVenda = Glass.Conversoes.StrParaIntNullable(tipoVendaStr);
    
                if (!retorno.Contains(tipoVenda) && !PedidoDAO.Instance.IsPedidoReposicao(null, idPedidoStr) && !PedidoDAO.Instance.IsPedidoGarantia(null, idPedidoStr))
                    retorno.Add(tipoVenda);
            }
    
            return retorno;
        }
    
        protected void btnRecalcular_Click(object sender, EventArgs e)
        {
            mensagemErro.Visible = false;
            
            /* Chamado 55590. */
            if (hdfRecarregarTabelaPedido != null && hdfRecarregarTabelaPedido.Value.ToLower() == "true")
            {
                grdPedido.DataBind();
                hdfRecarregarTabelaPedido.Value = string.Empty;
            }

            CalcularPrecos();
        }
    
        private class Contadores
        {
            public int Qtde = 0, QtdeMarcados = 0;
            public float Total = 0, TotM = 0, ValorIcms = 0;
            public List<int> Linhas = new List<int>();
            public bool Marcado = true;
    
            public string CodInterno, Descricao, Altura, Largura;
        }
    
        protected void IniciaTreeView(GridView grdProdutosPedido)
        {
            if ((!Liberacao.DadosLiberacao.LiberarPedidoProdutos || !grdProdutosPedido.Columns[10].Visible) &&
                !PedidoConfig.ExibirProdutosPedidoAoLiberar)
                return;
    
            var desabilitarCamposProduto = (!Liberacao.DadosLiberacao.LiberarPedidoProdutos || !grdProdutosPedido.Columns[10].Visible) &&
                PedidoConfig.ExibirProdutosPedidoAoLiberar && !Liberacao.DadosLiberacao.LiberarPedidoProdutos;
    
            var contadores = new Dictionary<uint, Contadores>();
    
            for (var i = 0; i < grdProdutosPedido.Rows.Count; i++)
            {
                var idProdPed = Glass.Conversoes.StrParaUint(((HiddenField)grdProdutosPedido.Rows[i].FindControl("hdfIdProdPed")).Value);
                if (!contadores.ContainsKey(idProdPed))
                {
                    contadores.Add(idProdPed, new Contadores());
    
                    contadores[idProdPed].CodInterno = ((Label)grdProdutosPedido.Rows[i].FindControl("lblCodInterno")).Text;
                    contadores[idProdPed].Descricao = ((Label)grdProdutosPedido.Rows[i].FindControl("lblDescricao")).Text;
                    contadores[idProdPed].Altura = ((Label)grdProdutosPedido.Rows[i].FindControl("lblAltura")).Text;
                    contadores[idProdPed].Largura = ((Label)grdProdutosPedido.Rows[i].FindControl("lblLargura")).Text;
                }
    
                var podeMarcar = ((CheckBox)grdProdutosPedido.Rows[i].FindControl("chkSelProdPed")).Enabled;
                var qtdeDisponivel = !podeMarcar ? 0 : 
                    !String.IsNullOrEmpty(((Label)grdProdutosPedido.Rows[i].FindControl("lblNumEtiqueta")).Text) ? 1 : 
                    Glass.Conversoes.StrParaInt(((Label)grdProdutosPedido.Rows[i].FindControl("lblQtdeDisp")).Text);
    
                var total = float.Parse(((Label)grdProdutosPedido.Rows[i].FindControl("lblTotal")).Text, System.Globalization.NumberStyles.Currency);
                var totM = Glass.Conversoes.StrParaFloat(((Label)grdProdutosPedido.Rows[i].FindControl("lblTotM")).Text);
                var valorIcms = float.Parse(((Label)grdProdutosPedido.Rows[i].FindControl("lblValorIcms")).Text, System.Globalization.NumberStyles.Currency);
                var marcado = ((CheckBox)grdProdutosPedido.Rows[i].FindControl("chkSelProdPed")).Checked;
    
                contadores[idProdPed].QtdeMarcados += marcado ? qtdeDisponivel : 0;
                contadores[idProdPed].Qtde += qtdeDisponivel;
                contadores[idProdPed].Total += total;
                contadores[idProdPed].TotM += totM;
                contadores[idProdPed].ValorIcms += valorIcms;
    
                contadores[idProdPed].Marcado = contadores[idProdPed].Marcado && marcado;
                contadores[idProdPed].Linhas.Add(i);
    
                if (_prodPedPesoM2 == null || _prodPedPesoM2.Count == 0)
                {
                    lblTotalM2.Text = "0.00";
                    lblTotalPeso.Text = "0.00";
                }
    
                if (marcado)
                {
                    lblDescrTotalM2.Visible = true;
                    lblTotalM2.Visible = true;
                    lblDescrTotalPeso.Visible = true;
                    lblTotalPeso.Visible = true;
    
                    if (!_prodPedPesoM2.Contains(idProdPed))
                    {
                        _prodPedPesoM2.Add(idProdPed);
    
                        var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(null, idProdPed);
                        var peso = Data.Helper.Utils.CalcPeso((int)idProd, ProdutoDAO.Instance.ObtemEspessura((int)idProd), totM, Glass.Conversoes.StrParaFloat(qtdeDisponivel.ToString()),
                            String.IsNullOrEmpty(contadores[idProdPed].Altura) ? Glass.Conversoes.StrParaFloat(contadores[idProdPed].Altura) : 0F, false);
    
                        lblTotalM2.Text = (Glass.Conversoes.StrParaFloat(lblTotalM2.Text) + totM).ToString("0.##");
                        lblTotalPeso.Text = (Glass.Conversoes.StrParaFloat(lblTotalPeso.Text) + peso).ToString("0.##");
                    }
                }
            }
        
            foreach (uint idProdPed in contadores.Keys)
            {
                var pchInicio = grdProdutosPedido.Rows[contadores[idProdPed].Linhas[0]].FindControl("pchInicio") as PlaceHolder;
                if (pchInicio == null)
                    continue;
    
                var linha = new HtmlGenericControl();
                linha.InnerHtml = @"
                        <span style='margin-left: -15px'>
                            <img id='exibir_" + idProdPed + @"' width='10px' src='../images/menos.gif' onclick='exibeProd(""" + grdProdutosPedido.ClientID + @""", " + idProdPed + @", this)' style='cursor: pointer;margin-left: -1px;margin-right: 1px;" + (desabilitarCamposProduto ? "visibility: hidden" : "") + @"' />
                            <input type='checkbox'" + (desabilitarCamposProduto ? " disabled='disabled'" : "") + @" id='chkProdPed_" + idProdPed + @"' onclick='marcaProd(""" + grdProdutosPedido.ClientID + @""", " + idProdPed + @", this.checked)'" + (contadores[idProdPed].Marcado ? " checked='checked'" : "") + @" />
                            <label for='chkProdPed_" + idProdPed + "'>" + contadores[idProdPed].Descricao + @"</label>
                        </span>
                        <script type='text/javascript'>
                            var celula = document.getElementById('chkProdPed_" + idProdPed + @"');
                            while (celula.nodeName.toLowerCase() != 'td')
                                celula = celula.parentNode;
        
                            celula.colSpan = 3;
                            celula.style.width = '';
                        </script>
                    <td>
                        " + contadores[idProdPed].Altura + @"
                    </td>
                    <td>
                        " + contadores[idProdPed].Largura + @"
                    </td>
                    <td>
                        " + contadores[idProdPed].TotM.ToString("0.##") + @"
                    </td>
                    " + (grdProdutosPedido.Columns[6].Visible ?
                        @"<td>
                            " + contadores[idProdPed].ValorIcms.ToString("C") + @"
                        </td>" : ""
                    ) +
                    @"<td>
                        " + contadores[idProdPed].Total.ToString("C") + @"
                    </td>
                    <td>
                        <input type='hidden' id='qtdeMax_" + idProdPed + @"' value='" + contadores[idProdPed].Qtde + @"' />
                        <input type='text'" + (desabilitarCamposProduto ? " disabled='disabled'" : "") + @" id='qtde_" + idProdPed + @"' value='" + contadores[idProdPed].QtdeMarcados + @"' onchange='alteraQtdeLib(""" + grdProdutosPedido.ClientID + @""", " + idProdPed + @")'
                            style='width: 30px' onkeypress='return soNumeros(event, true, true)'/> x
                        " + contadores[idProdPed].Qtde + @" produto" + (contadores[idProdPed].Qtde > 1 ? "s" : "") + @"
                    </td>
                </tr>
                <tr>
                    <td>";
    
                pchInicio.Controls.Add(linha);
                Page.ClientScript.RegisterStartupScript(GetType(), "exibirProd_" + idProdPed,
                    "document.getElementById('exibir_" + idProdPed + "').onclick();\n", true);
            }
        }
    
        private void AtualizaObsCliente()
        {
            if (hdfIdCliente.Value == "")
                return;
    
            lblObsCliente.ForeColor = Liberacao.TelaLiberacao.CorExibirObservacaoCliente;

            var obs = MetodosAjax.GetObsCli(hdfIdCliente.Value).Split(';');
            if (obs[0] != "Erro")
                lblObsCliente.Text = obs[1];
        }
    
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
    
        }
    
        private void AtualizaFormaPagto()
        {
            var idsPedidos = hdfBuscarIdsPedidos.Value;
    
            if (string.IsNullOrEmpty(idsPedidos))
            {
                drpFormaPagtoPrazo.Items.Clear();
                return;
            }

            var formasPagto = PedidoDAO.Instance.ObtemFormaPagto(idsPedidos);
    
            if (formasPagto != null && formasPagto.Count() > 0)
            {
                for (int i = 0; i < formasPagto.Count; i++)
                {
                    if (drpFormaPagtoPrazo.Items.FindByValue(formasPagto[i].ToString()) == null)
                    {
                        formasPagto.RemoveAt(i);
                        i--;
                    }
    
                    if (formasPagto.Count() == 0)
                        break;
                }
            }
    
            drpFormaPagtoPrazo.SelectedValue = formasPagto == null ? null :
                formasPagto.Distinct().Count() > 1 || formasPagto.Distinct().Count() == 0 ? null :
                formasPagto[0].ToString();
        }
    
        protected void drpFormaPagtoPrazo_DataBound(object sender, EventArgs e)
        {
            AtualizaFormaPagto();
        }
    }
}
