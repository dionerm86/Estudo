using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;
using Glass.Data.Helper;

namespace Glass.UI.Web.Cadastros
{
    public partial class CadReceberSinal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            hdfCxDiario.Value = Request["cxDiario"];
    
            if (!IsPostBack && Request["idPedido"] != null)
            {
                hdfBuscarIdsPedidos.Value = Request["idPedido"];
                grdPedido.DataBind();
            }
    
            if (Request["antecipado"] == "1")
            {
                Page.Title = "Pagamento Antecipado de Pedido";
                
                hdfIsSinal.Value = "false";
                btnReceberSinal.Visible = false;
                btnReceberPagtoAntecip.Visible = true;
            }
            else
            {
                hdfIsSinal.Value = "true";
                btnReceberSinal.Visible = true;
                btnReceberPagtoAntecip.Visible = false;
            }
    
            Ajax.Utility.RegisterTypeForAjax(typeof(Glass.UI.Web.Cadastros.CadReceberSinal));
            Ajax.Utility.RegisterTypeForAjax(typeof(MetodosAjax));
    
            if (hdfIdsPedidosRem.Value != String.Empty)
            {
                lblPedidosRem.Text = "Pedidos Removidos: " + hdfIdsPedidosRem.Value.TrimEnd(',');
                imbLimparRemovidos.Visible = true;
            }

            if (!string.IsNullOrWhiteSpace(hdfBuscarIdsPedidos.Value))
            {
                var idsLojas = PedidoDAO.Instance.ObtemIdsLojas(hdfBuscarIdsPedidos.Value);
                var lojas = LojaDAO.Instance.GetByString(idsLojas);

                // Esconde os dados sobre o ICMS se a empresa não calcular
                grdPedido.Columns[6].Visible = lojas.Any(f => f.CalcularIcmsPedido);
            }            
            
            grdPedido.Columns[10].Visible = PedidoConfig.Pedido_FastDelivery.FastDelivery;

            //Chamado 17870
            //Se utilizar o controle de comissão so pode receber sinal de um pedido por vez
            //Para que ao separar valores consiga referencia a NF-e na conta recebida gerada pelo sinal
            if (!IsPostBack && Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas &&
                /* Chamado 39027. */
                FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                btnBuscarPedidos.Style.Add("display","none");
                lblCliente.Style.Add("display", "none");
                txtNumCli.Style.Add("display", "none");
                txtNomeCliente.Style.Add("display", "none");
                lnkSelCliente.Style.Add("display", "none");
                lblData.Style.Add("display", "none");
                ctrlDataIni.Visible = false;
                ctrlDataFim.Visible = false;
            }

            if (!IsPostBack && !string.IsNullOrEmpty(Request["cxDiario"]))
                Page.Title = Page.Title + " (Caixa " + (Request["cxDiario"] == "1" ? "Diário" : "Geral") + ")";

            if (string.IsNullOrEmpty(Request["cxDiario"]) || Request["cxDiario"] != "1")
            {
                var login = UserInfo.GetUserInfo;

                // Recupera o funcionário
                var funcionario = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Global.Negocios.IFuncionarioFluxo>().ObtemFuncionario((int)login.CodUser);

                // Recupera os menus do sistema
                var fluxoMenu = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Glass.Global.Negocios.IMenuFluxo>();
                var menusFunc = fluxoMenu.ObterMenusPorFuncionario(funcionario);

                // Verifica se a pessoa tem acesso ao menu, se não tiver, redireciona para a tela padrão
                // Pega todos os menus da empresa e verifica se a página atual está listada nele, se tiver, verifica se o usuário tem acesso à ela
                var paginaAtual = Request.Url.ToString().ToLower();
                var idsMenu = fluxoMenu
                    .ObterMenusPorConfig((int)login.IdLoja)
                    .Where(f => !string.IsNullOrEmpty(f.Url) &&
                    paginaAtual.Contains(f.Url.ToLower().TrimStart('~', '/')))
                    .Select(f => f.IdMenu).ToList();

                if (idsMenu.Count > 0 && !menusFunc.Any(f => idsMenu.Contains(f.IdMenu)))
                {
                    Response.Redirect("CadReceberSinal.aspx?antecipado=1&cxDiario=1");
                    //Response.Redirect("~/WebGlass/Main.aspx");
                    return;
                }
            }
        }
    
        #region Métodos AJAX
    
        [Ajax.AjaxMethod]
        public string GetPedidosByCliente(string idCliente, string nomeCliente, string idsPedidosRem, string dataIni, string dataFim, string isSinalStr)
        {
            try
            {
                return PedidoDAO.Instance.GetIdsPedidosForReceberSinal(Glass.Conversoes.StrParaUint(idCliente), nomeCliente, idsPedidosRem, dataIni, dataFim, isSinalStr.ToLower() == "true");
            }
            catch
            {
                return "0";
            }
        }
    
        [Ajax.AjaxMethod]
        public string ValidaPedido(string idPedidoStr, string idClienteStr, string isSinalStr)
        {
            try
            {
                uint idPedido = Glass.Conversoes.StrParaUint(idPedidoStr);
    
                // Verifica se o pedido existe
                if (!PedidoDAO.Instance.PedidoExists(idPedido))
                    return "false|Não existe pedido com esse número.";
    
                if (!String.IsNullOrEmpty(idClienteStr) && Glass.Conversoes.StrParaUint(idClienteStr) != PedidoDAO.Instance.ObtemIdCliente(idPedido))
                    return "false|O cliente desse pedido é diferente do cliente do(s) pedido(s) já selecionado(s).";
                
                SinalDAO.Instance.ValidaSinalPedidos(idPedidoStr, isSinalStr.ToLower() == "true");
    
                return "true";
            }
            catch (Exception ex)
            {
                return "false|" + ex.Message;
            }
        }
    
        [Ajax.AjaxMethod]
        public string GetTotalPedidos(string idsPedidos)
        {
            uint id = 0;
            decimal total = 0;
            foreach (string s in idsPedidos.Split(','))
                if (uint.TryParse(s, out id))
                    total += PedidoDAO.Instance.ObtemValorEntrada(id);
    
            return total.ToString();
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
            TransacaoCapptaTefDAO.Instance.AtualizaPagamentosCappta(Data.Helper.UtilsFinanceiro.TipoReceb.SinalPedido, id.StrParaInt(),
                checkoutGuid, admCodes, customerReceipt, merchantReceipt, formasPagto);
        }

        /// <summary>
        /// Cancela o pagto que foi pago com TEF porem deu algum erro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="motivo"></param>
        [Ajax.AjaxMethod]
        public void CancelarSinalErroTef(string id, string motivo)
        {
            SinalDAO.Instance.CancelarComTransacao(id.StrParaUint(), null, false, false, "Falha no recebimento TEF. Motivo: " + motivo, DateTime.Now, true, false);
        }

        #endregion

        #region Confirma o recebimento

        [Ajax.AjaxMethod()]
        public string Confirmar(string idsPedidos, string dataRecebimento, string fPagtos, string valores, string contas, string depositoNaoIdentificado, string cartaoNaoIdentificado, string tpCartoes, 
            string gerarCredito, string creditoUtilizado, string cxDiario, string numAutConstrucard, string parcCredito, string chequesPagto, 
            string descontarComissao, string obs, string isSinalStr, string numAutCartao)
        {
            try
            {
                Glass.FilaOperacoes.ReceberSinal.AguardarVez();
                string[] sFormasPagto = fPagtos.Split(';');
                string[] sValoresReceb = valores.Split(';');
                string[] sIdContasBanco = contas.Split(';');
                string[] sTiposCartao = tpCartoes.Split(';');
                string[] sParcCartoes = parcCredito.Split(';');
                string[] sDepositoNaoIdentificado = depositoNaoIdentificado.Split(';');
                string[] sCartaoNaoIdentificado = cartaoNaoIdentificado.Split(';');
                
                string[] sNumAutCartao = numAutCartao.Split(';');

                uint[] formasPagto = new uint[sFormasPagto.Length];
                decimal[] valoresReceb = new decimal[sValoresReceb.Length];
                uint[] idContasBanco = new uint[sIdContasBanco.Length];
                uint[] tiposCartao = new uint[sTiposCartao.Length];
                uint[] parcCartoes = new uint[sParcCartoes.Length];
                uint[] depNaoIdentificado = new uint[sDepositoNaoIdentificado.Length];
                var cartNaoIdentificado = new uint[sCartaoNaoIdentificado.Length];

                for (int i = 0; i < sFormasPagto.Length; i++)
                {
                    formasPagto[i] = Conversoes.StrParaUint(sFormasPagto[i]);
                    valoresReceb[i] = Conversoes.StrParaDecimal(sValoresReceb[i]);
                    idContasBanco[i] = Conversoes.StrParaUint(sIdContasBanco[i]);
                    tiposCartao[i] = Conversoes.StrParaUint(sTiposCartao[i]);
                    parcCartoes[i] = Conversoes.StrParaUint(sParcCartoes[i]);
                    depNaoIdentificado[i] = !string.IsNullOrEmpty(sDepositoNaoIdentificado[i])
                        ? Convert.ToUInt32(sDepositoNaoIdentificado[i])
                        : 0;                  
                }

                for (int i = 0; i < sCartaoNaoIdentificado.Length; i++)
                {
                    cartNaoIdentificado[i] = !string.IsNullOrEmpty(sCartaoNaoIdentificado[i]) ? Convert.ToUInt32(sCartaoNaoIdentificado[i]) : 0;
                }

                decimal creditoUtil = Conversoes.StrParaDecimal(creditoUtilizado);

                bool isSinal = isSinalStr == "true";

                // Recebe Sinal/Confirma pedido
                string msg = SinalDAO.Instance.Receber(idsPedidos, dataRecebimento, valoresReceb, formasPagto,
                    idContasBanco, depNaoIdentificado, cartNaoIdentificado, tiposCartao,
                    gerarCredito == "true", creditoUtil, cxDiario == "1", numAutConstrucard, parcCartoes, chequesPagto,
                    descontarComissao == "true", obs, isSinal, sNumAutCartao);

                return "ok\t" + msg;
            }
            catch (Exception ex)
            {
                return "Erro\t" + MensagemAlerta.FormatErrorMsg(null, ex);
            }
            finally
            {
                FilaOperacoes.ReceberSinal.ProximoFila();
            }
        }
    
        #endregion
    
        #region Métodos usados na página
    
        private bool corAlternada = true;
    
        protected string GetAlternateClass()
        {
            corAlternada = !corAlternada;
            return corAlternada ? "alt" : "";
        }
    
        #endregion
    
        protected void ctrlFormaPagto1_Load(object sender, EventArgs e)
        {
            ctrlFormaPagto1.ParentID = divInsertSinal.ClientID;
            ctrlFormaPagto1.CampoCredito = hdfValorCredito;
            ctrlFormaPagto1.CampoValorConta = hdfValorASerPago;
            ctrlFormaPagto1.ExibirDataRecebimento = true;
            ctrlFormaPagto1.CampoClienteID = hdfIdCliente;
            ctrlFormaPagto1.UsarCreditoMarcado = FinanceiroConfig.OpcaoUsarCreditoMarcadaPagamentoAntecipadoPedido;

            if (!IsPostBack)
                ctrlFormaPagto1.DataRecebimento = DateTime.Now;
    
            // Esconde a mensagem de data retroativa se não utilizar a data de recebimento
            lblMensagemRetroativa.Visible = ctrlFormaPagto1.ExibirDataRecebimento;
        }
    
        protected void btnBuscarPedidos_Click(object sender, EventArgs e)
        {
            grdPedido.DataBind();
        }
    
        protected void imbLimparRemovidos_Click(object sender, ImageClickEventArgs e)
        {
            hdfBuscarIdsPedidos.Value += "," + hdfIdsPedidosRem.Value.Trim(',');
            hdfIdsPedidosRem.Value = String.Empty;
            lblPedidosRem.Text = "";
            imbLimparRemovidos.Visible = false;
    
            grdPedido.DataBind();
        }
    
        protected void grdPedido_DataBound(object sender, EventArgs e)
        {
            divInsertSinal.Visible = grdPedido.Rows.Count > 0;
            decimal totalPagar = 0;
            decimal valorCredito = 0;
    
            if (grdPedido.Rows.Count > 0)
            {
                hdfIdCliente.Value = ((HiddenField)grdPedido.Rows[0].FindControl("hdfIdCliente")).Value;
    
                // Calcula o total a ser pago
                string nomeCampo = Request["antecipado"] == "1" ? "hdfTotal" : "hdfValorEntrada";
                for (int i = 0; i < grdPedido.Rows.Count; i++)
                {
                    decimal totalPedidoPagar = Glass.Conversoes.StrParaDecimal(((HiddenField)grdPedido.Rows[i].FindControl(nomeCampo)).Value);
    
                    // Se for pagamento antecipado o valor da entrada do pedido deve ser desconsiderado no valor a ser pago.
                    if (Request["antecipado"] == "1")
                    {
                        uint idPedido = Glass.Conversoes.StrParaUint(((HiddenField)grdPedido.Rows[i].FindControl("hdfIdPedido")).Value);
                        var recebeuSinal = !PedidoDAO.Instance.TemSinalReceber(idPedido) && PedidoDAO.Instance.ObtemIdSinal(idPedido) > 0;
    
                        totalPagar += totalPedidoPagar - (recebeuSinal ? PedidoDAO.Instance.ObtemValorEntrada(idPedido) : 0);
                    }
                    else
                        totalPagar += totalPedidoPagar;
                }
    
                // Recupera o crédito do cliente
                valorCredito = ClienteDAO.Instance.GetCredito(Glass.Conversoes.StrParaUint(hdfIdCliente.Value));
            }
    
            hdfValorASerPago.Value = totalPagar.ToString("0.##");
            hdfValorCredito.Value = valorCredito.ToString("0.##");
        }
    }
}
