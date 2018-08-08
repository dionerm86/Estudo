using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.Helper;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlFormaPagto : BaseUserControl
    {
        #region Campos privados
    
        private int _numPossibilidadesPagto = PedidoConfig.FormaPagamento.NumeroFormasPagto;
        private bool _cadastrarCheque = false;
        private bool _selecionarCheque = false;
        private bool _recalcularCredito = false;
        private string _chequesQueryString;
        private string _chequesUrl;
        private string _cssStyleCabecalho;
        private string _cssStyleLinha;
        private string _cssStyleLinhaAlternada;
        private string _parentId;
        private string _callbackGerarCredito;
        private string _callbackRecebParcial;
        private string _callbackUsarCredito;
        private string _callbackSelCliente;
        private string _recebParcialDados;
        private bool _exibirNumBoleto = false;
        private bool _exibirTiposBoleto = FormaPagamento.ControleFormaPagamento.ExibirTiposBoleto;
        private bool _exibirGerarCredito = true;
        private bool _exibirUsarCredito = true;
        private bool _exibirCredito = true;
        private bool _exibirCliente = false;
        private bool _efetuarBindContaBanco = true;
        private bool _exibirApenasCartaoDebito = false;
        private string _textoValorReceb = "Valor Receb.";
        private string _tipoModel;
        private bool _exibirComissaoComissionado = PedidoConfig.Comissao.ComissaoPedido;
        private string _callbackTotal = null;
        private bool _cobrarJurosCartaoClientes = true;
        private bool _isRecebimento = true;
        private string _callbackIncluirCheque = "";
        private string _callbackExcluirCheque = "";
        private bool _bloquearCamposContaVazia = true;
        private bool _permitirValorPagarNegativo = false;
        
        private Control _campoCredito;
        private Control _campoValorConta;
        private Control _campoValorDesconto;
        private Control _campoTipoDesconto;
        private Control _campoValorAcrescimo;
        private Control _campoTipoAcrescimo;
        private Control _campoIdCliente;
        private Control _campoIdFornecedor;
        private Control _campoValorObra;
    
        #endregion
    
        #region Propriedades
    
        public bool PermitirValorPagarNegativo
        {
            get { return _permitirValorPagarNegativo; }
            set { _permitirValorPagarNegativo = value; }
        }
    
        /// <summary>
        /// Define se o valor do Crédito deve ser recalculado via javascript.
        /// </summary>
        public bool RecalcularCredito
        {
            get { return _recalcularCredito; }
            set { _recalcularCredito = value; }
        }
    
        /// <summary>
        /// Os campos devem ser bloqueados se o valor da conta for vazio?
        /// </summary>
        public bool BloquearCamposContaVazia
        {
            get { return _bloquearCamposContaVazia; }
            set { _bloquearCamposContaVazia = value; }
        }
    
        /// <summary>
        /// O controle será usado para recebimento?
        /// (Caso contrário será usado para pagamento)
        /// </summary>
        public bool IsRecebimento
        {
            get { return _isRecebimento; }
            set { _isRecebimento = value; }
        }
    
        /// <summary>
        /// Os juros dos cartões serão cobrados dos clientes, se possível?
        /// </summary>
        public bool CobrarJurosCartaoClientes
        {
            get { return _cobrarJurosCartaoClientes; }
            set { _cobrarJurosCartaoClientes = value; }
        }
    
        /// <summary>
        /// O Label com o valor restante deve ser exibido?
        /// </summary>
        public bool ExibirValorRestante
        {
            get { return lblRestante.Visible; }
            set { lblRestante.Visible = value; }
        }
    
        /// <summary>
        /// Define que apenas cartão de débito será exibido ao selecionar a forma de pagamento cartão
        /// </summary>
        public bool ExibirApenasCartaoDebito 
        {
            get  { return _exibirApenasCartaoDebito && FinanceiroConfig.FormaPagamento.ExibirApenasCartaoDebito; }
            set { _exibirApenasCartaoDebito = value; }
        }
    
        /// <summary>
        /// Retorna/altera o ID do cliente.
        /// </summary>
        public uint IdCliente
        {
            get { return !string.IsNullOrEmpty(txtNumCli.Text) ? Glass.Conversoes.StrParaUint(txtNumCli.Text) : 0; }
            set { txtNumCli.Text = value.ToString(); }
        }
    
        /// <summary>
        /// Retorna/altera o nome do cliente.
        /// </summary>
        public string NomeCliente
        {
            get { return txtNomeCliente.Text; }
            set { txtNomeCliente.Text = value; }
        }
    
        /// <summary>
        /// Função de callback que deve ser executada ao calcular o valor total pago.
        /// </summary>
        public string CallbackTotal
        {
            get { return !string.IsNullOrEmpty(_callbackTotal) ? "'" + _callbackTotal + "'" : "''"; }
            set { _callbackTotal = value; }
        }
    
        /// <summary>
        /// Função de callback que deve ser executada ao selecionar o cliente.
        /// </summary>
        public string CallbackSelCliente
        {
            get { return !string.IsNullOrEmpty(_callbackSelCliente) ? "'" + _callbackSelCliente + "'" : "''"; }
            set { _callbackSelCliente = value; }
        }
    
        /// <summary>
        /// O campo 'Usar crédito' deve vir marcado por padrão?
        /// </summary>
        public bool UsarCreditoMarcado
        {
            get { return chkUsarCredito.Checked; }
            set { chkUsarCredito.Checked = value; }
        }
    
        /// <summary>
        /// O tipo da model usado para o cálculo da comissão.
        /// </summary>
        public string TipoModel
        {
            get { return !string.IsNullOrEmpty(_tipoModel) ? _tipoModel : ""; }
            set { _tipoModel = value; }
        }
    
        /// <summary>
        /// Método usado para listar as formas de pagamento.
        /// </summary>
        public string MetodoFormasPagto
        {
            get { return odsFormaPagto.SelectMethod; }
            set { odsFormaPagto.SelectMethod = value; }
        }
    
        /// <summary>
        /// Texto que aparecerá no controle antes dos valores.
        /// </summary>
        public string TextoValorReceb
        {
            get { return _textoValorReceb; }
            set { _textoValorReceb = !string.IsNullOrEmpty(value) ? value : "Valor Receb."; }
        }
    
        /// <summary>
        /// O Label com o valor a pagar deve ser exibido?
        /// </summary>
        public bool ExibirValorAPagar
        {
            get { return lblValorASerPago.Visible; }
            set 
            {
                lblTextoValorASerPago.Visible = value;
                lblValorASerPago.Visible = value;
            }
        }
    
        /// <summary>
        /// Define o número de "controles" de forma de pagamento serão criados.
        /// </summary>
        public int NumPossibilidadesPagto
        {
            get { return _numPossibilidadesPagto; }
            set { _numPossibilidadesPagto = value; }
        }
    
        /// <summary>
        /// O cheque deve ser cadastrado no banco de dados ao ser inserido?
        /// </summary>
        public bool CadastrarCheque
        {
            get { return _cadastrarCheque; }
            set { _cadastrarCheque = value; }
        }
    
        /// <summary>
        /// O cheque deve ser selecionado ao invés de cadastrado?
        /// </summary>
        public bool SelecionarCheque
        {
            get { return _selecionarCheque; }
            set { _selecionarCheque = value; }
        }
    
        /// <summary>
        /// Função de JavaScript que retorna o QueryString usado para abrir a janela de cadastro de cheques.
        /// </summary>
        public string FuncaoQueryStringCheques
        {
            get { return !string.IsNullOrEmpty(_chequesQueryString) ? _chequesQueryString + "()" : "''"; }
            set { _chequesQueryString = value; }
        }
    
        /// <summary>
        /// Função de JavaScript que retorna a URL usada para abrir a janela de cadastro de cheques.
        /// </summary>
        public string FuncaoUrlCheques
        {
            get { return _chequesUrl; }
            set { _chequesUrl = value; }
        }
    
        /// <summary>
        /// Função de JavaScript executada como callback ao incluir um cheque.
        /// </summary>
        public string CallbackIncluirCheque
        {
            get { return "'" + _callbackIncluirCheque + "'"; }
            set { _callbackIncluirCheque = value; }
        }
    
        /// <summary>
        /// Função de JavaScript executada como callback ao excluir um cheque.
        /// </summary>
        public string CallbackExcluirCheque
        {
            get { return "'" + _callbackExcluirCheque + "'"; }
            set { _callbackExcluirCheque = value; }
        }
    
        /// <summary>
        /// Função de JavaScript que retorna os dados usados pela função de recebimento parcial.
        /// </summary>
        public string FuncaoDadosRecebParcial
        {
            get { return !string.IsNullOrEmpty(_recebParcialDados) ? _recebParcialDados : ""; }
            set { _recebParcialDados = value; }
        }
    
        /// <summary>
        /// Classe CSS aplicado aos cabeçalhos do controle.
        /// </summary>
        public string CssStyleCabecalho
        {
            get { return _cssStyleCabecalho; }
            set { _cssStyleCabecalho = value; }
        }
    
        /// <summary>
        /// Classe CSS aplicado às linhas do controle.
        /// </summary>
        public string CssStyleLinha
        {
            get { return _cssStyleLinha; }
            set { _cssStyleLinha = value; }
        }
    
        /// <summary>
        /// Classe CSS aplicado às linhas alternadas do controle.
        /// </summary>
        public string CssStyleLinhaAlternada
        {
            get { return _cssStyleLinhaAlternada; }
            set { _cssStyleLinhaAlternada = value; }
        }
    
        /// <summary>
        /// Identificador do controle pai.
        /// </summary>
        public string ParentID
        {
            get { return _parentId; }
            set { _parentId = value; }
        }
    
        /// <summary>
        /// Função executada quando o CheckBox chkGerarCredito for clicado.
        /// </summary>
        public string CallbackGerarCredito
        {
            get { return !String.IsNullOrEmpty(_callbackGerarCredito) ? _callbackGerarCredito : ""; }
            set { _callbackGerarCredito = value; }
        }
    
        /// <summary>
        /// Função executada quando o CheckBox chkRecebimentoParcial for clicado.
        /// </summary>
        public string CallbackRecebimentoParcial
        {
            get { return !String.IsNullOrEmpty(_callbackRecebParcial) ? _callbackRecebParcial : ""; }
            set { _callbackRecebParcial = value; }
        }
    
        /// <summary>
        /// Função executada quando o CheckBox chkUsarCredito for clicado.
        /// </summary>
        public string CallbackUsarCredito
        {
            get { return !String.IsNullOrEmpty(_callbackUsarCredito) ? _callbackUsarCredito : ""; }
            set { _callbackUsarCredito = value; }
        }
    
        /// <summary>
        /// Data de recebimento escolhida.
        /// </summary>
        public DateTime? DataRecebimento
        {
            get { return !String.IsNullOrEmpty(txtDataRecebimento.Text) ? (DateTime?)DateTime.Parse(txtDataRecebimento.Text) : null; }
            set { txtDataRecebimento.Text = value != null ? value.Value.ToString("dd/MM/yyyy") : ""; }
        }
    
        /// <summary>
        /// Juros digitados no controle.
        /// </summary>
        public decimal Juros
        {
            get { return Glass.Conversoes.StrParaDecimal(txtJuros.Text); }
            set { txtJuros.Text = value.ToString().Replace('.', ','); }
        }
    
        /// <summary>
        /// O troco deve ser calculado e exibido?
        /// </summary>
        public bool CalcularTroco
        {
            get { return lblTroco.Visible; }
            set { lblTroco.Visible = value; }
        }
    
        /// <summary>
        /// A opção de receber a comissão do comissionado deve ser exibida?
        /// </summary>
        public bool ExibirComissaoComissionado
        {
            get { return _exibirComissaoComissionado; }
            set { _exibirComissaoComissionado = PedidoConfig.Comissao.ComissaoPedido && value; }
        }
    
        /// <summary>
        /// O número do boleto deve ser exibido?
        /// </summary>
        public bool ExibirNumBoleto
        {
            get { return _exibirNumBoleto; }
            set { _exibirNumBoleto = value; }
        }
    
        /// <summary>
        /// Os tipos do boleto devem ser exibidos?
        /// </summary>
        public bool ExibirTiposBoleto
        {
            get { return _exibirTiposBoleto; }
            set { _exibirTiposBoleto = value; }
        }
    
        /// <summary>
        /// O campo de data de recebimento deve ser exibido?
        /// </summary>
        public bool ExibirDataRecebimento
        {
            get { return txtDataRecebimento.Visible; }
            set { txtDataRecebimento.Visible = value; }
        }
    
        /// <summary>
        /// O campo de juros deve ser exibido?
        /// </summary>
        public bool ExibirJuros
        {
            get { return txtJuros.Visible; }
            set { txtJuros.Visible = value; }
        }
    
        /// <summary>
        /// O CheckBox 'Usar Crédito' deve ser exibido?
        /// </summary>
        public bool ExibirUsarCredito
        {
            get { return _exibirUsarCredito; }
            set { _exibirUsarCredito = value; }
        }
    
        /// <summary>
        /// O CheckBox 'Gerar Crédito' deve ser exibido?
        /// </summary>
        public bool ExibirGerarCredito
        {
            get { return _exibirGerarCredito; }
            set { _exibirGerarCredito = value; }
        }
    
        /// <summary>
        /// O Label com o valor de crédito deve ser exibido?
        /// </summary>
        public bool ExibirCredito
        {
            get { return _exibirCredito; }
            set { _exibirCredito = value; }
        }
    
        /// <summary>
        /// O campo com a seleção do cliente deve ser exibido?
        /// </summary>
        public bool ExibirCliente
        {
            get { return _exibirCliente; }
            set 
            {
                _exibirCliente = value && _campoCredito == null;
                hdfExibirCliente.Value = _exibirCliente.ToString();
            }
        }
    
        /// <summary>
        /// Deve ser feito bind na drpContaBanco?
        /// </summary>
        public bool EfetuarBindContaBanco
        {
            get { return _efetuarBindContaBanco; }
            set { _efetuarBindContaBanco = value; }
        }
    
        /// <summary>
        /// O CheckBox 'Receb. Parcial' deve ser exibido?
        /// </summary>
        public bool ExibirRecebParcial
        {
            get { return chkRecebimentoParcial.Visible; }
            set { chkRecebimentoParcial.Visible = value; }
        }
    
        /// <summary>
        /// Os valores para cada pagamento.
        /// </summary>
        public decimal[] Valores
        {
            get
            {
                decimal[] retorno = new decimal[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtValor = (TextBox)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "txtValor");
                    retorno[i] = Glass.Conversoes.StrParaDecimal(txtValor.Text);
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtValor = (TextBox)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "txtValor");
                    txtValor.Text = value[i].ToString().Replace(".", ",");
                }
            }
        }
    
        /// <summary>
        /// As formas de pagamento escolhidas para cada pagamento.
        /// </summary>
        public uint[] FormasPagto
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpFormaPagto = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpFormaPagto");
                    retorno[i] = drpFormaPagto.SelectedValue != "" ? Glass.Conversoes.StrParaUint(drpFormaPagto.SelectedValue) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpFormaPagto = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpFormaPagto");
                    drpFormaPagto.SelectedValue = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// Os tipos de cartão escolhidos para cada pagamento.
        /// </summary>
        public uint[] TiposCartao
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpTipoCartao = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpTipoCartao");
                    retorno[i] = drpTipoCartao.SelectedValue != "" ? Glass.Conversoes.StrParaUint(drpTipoCartao.SelectedValue) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpTipoCartao = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpTipoCartao");
                    drpTipoCartao.SelectedValue = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// O número de parcelas do cartão de crédito para cada pagamento.
        /// </summary>
        public uint[] ParcelasCartao
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpParcCredito = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpParcCredito");
                    retorno[i] = drpParcCredito.SelectedValue != "" ? Conversoes.StrParaUint(drpParcCredito.SelectedValue) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpParcCredito = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpParcCredito");
                    drpParcCredito.SelectedValue = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// Os tipos de boleto escolhidos para cada pagamento.
        /// </summary>
        public uint[] TiposBoleto
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpTipoBoleto = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpTipoBoleto");
                    retorno[i] = drpTipoBoleto != null && drpTipoBoleto.SelectedValue != "" ? Conversoes.StrParaUint(drpTipoBoleto.SelectedValue) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpTipoBoleto = (DropDownList)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "drpTipoBoleto");
                    drpTipoBoleto.SelectedValue = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// A string com os dados dos cheques.
        /// </summary>
        public string ChequesString
        {
            get { return hdfCheques.Value.Trim(' ', '|', ','); }
            set { /* Não faz nada */ }
        }
    
        /// <summary>
        /// Os cheques cadastrados como pagamento.
        /// </summary>
        public Cheques[] Cheques
        {
            get
            {
                // Garante que haja um cheque selecionado pelo usuário
                if (ChequesString == "")
                    return new Cheques[0];
    
                // Verifica se o controle faz seleção de cheques
                if (_selecionarCheque)
                    return ChequesDAO.Instance.GetByPks(ChequesString.Replace("|", ","));
                else
                {
                    // Verifica se os cheques já foram cadastrados
                    if (_cadastrarCheque)
                        return new Cheques[0];
    
                    // Cria os vetores com os dados dos cheques e de retorno
                    string[] cheques = ChequesString.Split('|');
                    Cheques[] retorno = new Cheques[cheques.Length];
    
                    for (int i = 0; i < cheques.Length; i++)
                        retorno[i] = ChequesDAO.Instance.GetFromString(cheques[i]);
    
                    // Retorna o vetor de cheques
                    return retorno;
                }
            }
        }
    
        /// <summary>
        /// As contas bancárias escolhidas como pagamento.
        /// </summary>
        public uint[] ContasBanco
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpContaBanco = (DropDownList)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "drpContaBanco");
                    retorno[i] = drpContaBanco.SelectedValue != "" ? Conversoes.StrParaUint(drpContaBanco.SelectedValue) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    DropDownList drpContaBanco = (DropDownList)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "drpContaBanco");
                    drpContaBanco.SelectedValue = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// Os valores das taxas de antecipação para os boletos.
        /// </summary>
        public decimal[] TaxasAntecipacao
        {
            get
            {
                decimal[] retorno = new decimal[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtTaxaAntecipacao = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtTaxaAntecipacao");
                    retorno[i] = txtTaxaAntecipacao != null ? Conversoes.StrParaDecimal(txtTaxaAntecipacao.Text) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtTaxaAntecipacao = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtTaxaAntecipacao");
                    txtTaxaAntecipacao.Text = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// Os números dos Construcards para cada pagamento.
        /// </summary>
        public string NumAutConstrucard
        {
            get
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    uint? formaPagto = FormasPagto[i];
                    if (formaPagto != null && formaPagto.Value == (uint)Pagto.FormaPagto.Construcard)
                    {
                        TextBox txtNumAutConstrucard = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumAutConstrucard");
                        return !String.IsNullOrEmpty(txtNumAutConstrucard.Text) ? txtNumAutConstrucard.Text : null;
                    }
                }
    
                return null;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    uint? formaPagto = FormasPagto[i];
                    if (formaPagto != null && formaPagto.Value == (uint)Pagto.FormaPagto.Construcard)
                    {
                        TextBox txtNumAutConstrucard = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumAutConstrucard");
                        txtNumAutConstrucard.Text = value;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Os numeros de autorização dos cartoes utilizados
        /// </summary>
        public string[] NumAutCartao
        {
            get
            {
                string[] retorno = new string[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtNumAutCartao = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumAutCartao");
                    retorno[i] = txtNumAutCartao != null ? txtNumAutCartao.Text : "";
                }

                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtNumAutCartao = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumAutCartao");
                    txtNumAutCartao.Text = value[i];
                }
            }
        }

        /// <summary>
        /// Os números dos boletos para cada pagamento.
        /// </summary>
        public string[] NumerosBoleto
        {
            get
            {
                string[] retorno = new string[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtNumeroBoleto = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumeroBoleto");
                    retorno[i] = txtNumeroBoleto != null ? txtNumeroBoleto.Text : null;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtNumeroBoleto = (TextBox)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "txtNumeroBoleto");
                    txtNumeroBoleto.Text = value[i];
                }
            }
        }
    
        /// <summary>
        /// Os depósitos não identificados escolhidos para cada pagamento.
        /// </summary>
        public uint[] DepositosNaoIdentificados
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    ctrlSelPopup selDni = (ctrlSelPopup)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "selDni");
                    retorno[i] = selDni != null && selDni.Valor != "" ? Conversoes.StrParaUint(selDni.Valor) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    ctrlSelPopup selDni = (ctrlSelPopup)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "selDni");
                    selDni.Valor = value[i].ToString();
                }
            }
        }

        /// <summary>
        /// Os cartões não identificados escolhidos para cada pagamento.
        /// </summary>
        public uint[] CartoesNaoIdentificados
        {
            get
            {
                uint trocador = 0;

                if (string.IsNullOrEmpty(hdfCNI.Value))
                    return new uint[] { };

                return (hdfCNI.Value.Split(';').Select(f => uint.TryParse(f, out trocador))
                    .Select(f => trocador)).Where(f => f > 0).ToArray();
            }
        }

        /// <summary>
        /// As antecipações de fornecedores escolhidos para cada pagamento.
        /// </summary>
        public uint[] AntecipacoesFornecedores
        {
            get
            {
                uint[] retorno = new uint[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    ctrlSelPopup selAntecipFornec = (ctrlSelPopup)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "selAntecipFornec");
                    retorno[i] = selAntecipFornec != null && selAntecipFornec.Valor != "" ? Glass.Conversoes.StrParaUint(selAntecipFornec.Valor) : 0;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    ctrlSelPopup selAntecipFornec = (ctrlSelPopup)GetLinhaBaixo(i + 1).FindControl(PrefixoControles(i + 1) + "selAntecipFornec");
                    selAntecipFornec.Valor = value[i].ToString();
                }
            }
        }
    
        /// <summary>
        /// As datas de recebimento para cada pagamento.
        /// </summary>
        public DateTime?[] DatasFormasPagamento
        {
            get
            {
                DateTime?[] retorno = new DateTime?[_numPossibilidadesPagto];
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtData = (TextBox)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "txtData");
                    retorno[i] = txtData != null && !string.IsNullOrEmpty(txtData.Text) ? (DateTime?)DateTime.Parse(txtData.Text) : null;
                }
    
                return retorno;
            }
            set
            {
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                {
                    TextBox txtData = (TextBox)GetLinhaCima(i + 1).FindControl(PrefixoControles(i + 1) + "txtData");
                    if (txtData != null)
                        txtData.Text = value[i] != null ? value[i].Value.ToString("dd/MM/yyyy") : "";
                }
            }
        }
    
        /// <summary>
        /// Retorna o valor da conta.
        /// </summary>
        public decimal ValorConta
        {
            get { return Glass.Conversoes.StrParaDecimal(GetValueFromControl(CampoValorConta)); }
        }
    
        /// <summary>
        /// Retorna o valor pago.
        /// </summary>
        public decimal ValorPago
        {
            get
            {
                decimal valorPago = 0;
                foreach (decimal v in Valores)
                    valorPago += v;
    
                return valorPago;
            }
        }
    
        /// <summary>
        /// Retorna o valor do acréscimo.
        /// </summary>
        public decimal ValorAcrescimo
        {
            get 
            {
                string tipo = GetValueFromControl(CampoTipoAcrescimo);
                decimal acrescimo = Glass.Conversoes.StrParaDecimal(GetValueFromControl(CampoValorAcrescimo));
    
                if (tipo != null)
                    return tipo == "%" || tipo == "1" ? ValorConta * acrescimo / 100 : acrescimo;
                else
                    return acrescimo;
            }
        }
    
        /// <summary>
        /// Retorna o valor do desconto.
        /// </summary>
        public decimal ValorDesconto
        {
            get
            {
                string tipo = GetValueFromControl(CampoTipoDesconto);
                decimal desconto = Glass.Conversoes.StrParaDecimal(GetValueFromControl(CampoValorDesconto));
    
                if (tipo != null)
                    return tipo == "%" || tipo == "1" ? ValorConta * desconto / 100 : desconto;
                else
                    return desconto;
            }
        }
    
        /// <summary>
        /// Retorna o valor a pagar.
        /// </summary>
        public decimal ValorPagar
        {
            get { return ValorConta + ValorAcrescimo - ValorDesconto + Juros; }
        }
    
        /// <summary>
        /// Retorna o valor da comissão que será descontado.
        /// </summary>
        public decimal ValorComissao
        {
            get { return chkComissaoComissionado.Checked ? Glass.Conversoes.StrParaDecimal(GetValueFromControl(txtValorComissao)) : 0; }
        }
    
        /// <summary>
        /// Retorna o valor do crédito.
        /// </summary>
        public decimal ValorCredito
        {
            get { return !ExibirCliente ? Glass.Conversoes.StrParaDecimal(GetValueFromControl(CampoCredito)) : Glass.Conversoes.StrParaDecimal(hdfFormaPagtoCreditoCliente.Value); }
        }
    
        /// <summary>
        /// Retorna/altera o valor do crédito utilizado pelo controle.
        /// </summary>
        public decimal CreditoUtilizado
        {
            get { return UsarCreditoMarcado ? Glass.Conversoes.StrParaDecimal(txtCreditoUtilizado.Text) : 0; }
            set { txtCreditoUtilizado.Text = value.ToString(); }
        }
    
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set
            {
                // Atualiza o ViewState do controle
                base.EnableViewState = value;
    
                // Atualiza os ViewStates dos controles internos
                for (int i = 0; i < tblFormaPagto.Rows.Count; i++)
                    for (int j = 0; j < tblFormaPagto.Rows[i].Cells.Count; j++)
                        foreach (Control c in tblFormaPagto.Rows[i].Cells[j].Controls)
                            c.EnableViewState = value;
            }
        }

        public bool GerarCredito { get { return chkGerarCredito.Checked; } }

        #endregion

        #region Propriedades de controles da página

        /// <summary>
        /// Campo com o ID do cliente.
        /// </summary>
        public Control CampoClienteID
        {
            get { return _campoIdCliente; }
            set { _campoIdCliente = value; }
        }
    
        /// <summary>
        /// Campo com o ID do fornecedor.
        /// </summary>
        public Control CampoFornecedorID
        {
            get { return _campoIdFornecedor; }
            set { _campoIdFornecedor = value; }
        }
    
        /// <summary>
        /// Campo com o crédito que o cliente possui.
        /// </summary>
        public Control CampoCredito
        {
            get { return _campoCredito; }
            set { _campoCredito = value; }
        }
    
        /// <summary>
        /// Campo com o valor da conta que será paga.
        /// </summary>
        public Control CampoValorConta
        {
            get { return _campoValorConta; }
            set { _campoValorConta = value; }
        }
    
        /// <summary>
        /// Campo com o valor do desconto.
        /// </summary>
        public Control CampoValorDesconto
        {
            get { return _campoValorDesconto; }
            set { _campoValorDesconto = value; }
        }
    
        /// <summary>
        /// Campo com o tipo do desconto.
        /// </summary>
        public Control CampoTipoDesconto
        {
            get { return _campoTipoDesconto; }
            set { _campoTipoDesconto = value; }
        }
    
        /// <summary>
        /// Campo com o valor do acréscimo.
        /// </summary>
        public Control CampoValorAcrescimo
        {
            get { return _campoValorAcrescimo; }
            set { _campoValorAcrescimo = value; }
        }
    
        /// <summary>
        /// Campo com o tipo do acréscimo.
        /// </summary>
        public Control CampoTipoAcrescimo
        {
            get { return _campoTipoAcrescimo; }
            set { _campoTipoAcrescimo = value; }
        }
    
        /// <summary>
        /// Campos com o valor da obra.
        /// </summary>
        public Control CampoValorObra
        {
            get { return _campoValorObra; }
            set { _campoValorObra = value; }
        }
    
        #endregion
    
        #region Métodos de suporte
    
        /// <summary>
        /// Retorna as formas de pagamento disponíveis para o controle.
        /// </summary>
        /// <returns></returns>
        private string GetFormasPagtoDisponiveis()
        {
            string retorno = "";
            foreach (FormaPagto f in (odsFormaPagto.Select() as FormaPagto[]))
            {
                if (f.IdFormaPagto.GetValueOrDefault(0) == 14 && !MenuConfig.ExibirCartaoNaoIdentificado)
                    continue;

                retorno += f.IdFormaPagto.GetValueOrDefault(0) + ",";
    
                if (IsRecebimento && f.IdFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito && DepositoNaoIdentificadoDAO.Instance.PossuiNaoUtilizados())
                    retorno += (uint)Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado + ",";

                if (IsRecebimento && f.IdFormaPagto == (uint)Pagto.FormaPagto.Cartao)
                    retorno += (uint)Pagto.FormaPagto.CartaoNaoIdentificado + ",";
            }
    
            return retorno.TrimEnd(',');
        }
    
        /// <summary>
        /// Retorna a URL usada para a tela dos cheques, de acordo com o número do pagamento.
        /// </summary>
        /// <param name="numPagto">O número do pagamento do controle.</param>
        /// <returns>Uma string com a função JavaScript que retorna a URL dos cheques.</returns>
        private string GetUrlCheques(int numPagto, bool incluirPadraoRetorno)
        {
            string urlPadrao = this.ResolveClientUrl(!_selecionarCheque ? "~/Cadastros/CadCheque.aspx" : "~/Utils/SelCheque.aspx");
            return (string.IsNullOrEmpty(_chequesUrl) ? "'" + urlPadrao + "'" :
                _chequesUrl + "(getVar('" + this.ClientID + "').FormasPagamento(false)[" + numPagto + "], '" + urlPadrao + "')") +
                (incluirPadraoRetorno ? ", '" + urlPadrao + "'" : "");
        }
    
        /// <summary>
        /// Retorna o prefixo dos controles.
        /// </summary>
        /// <param name="numPagto">O número do pagamento do controle.</param>
        /// <returns>Uma string usada como prefixo para os nomes dos controles.</returns>
        private string PrefixoControles(int numPagto)
        {
            return tblFormaPagto.ID + "_Pagto" + numPagto + "_";
        }
    
        /// <summary>
        /// Retorna a linha de cima da tabela de um pagamento.
        /// </summary>
        /// <param name="numPagto">O número do pagamento.</param>
        /// <returns>Um objeto TableRow.</returns>
        private TableRow GetLinhaCima(int numPagto)
        {
            return ((Table)tblFormaPagto.Rows[(numPagto - 1) * 2].Cells[0].Controls[0]).Rows[0];
        }
    
        /// <summary>
        /// Retorna a linha de baixo da tabela de um pagamento.
        /// </summary>
        /// <param name="numPagto">O número do pagamento.</param>
        /// <returns>Um objeto TableRow.</returns>
        private TableRow GetLinhaBaixo(int numPagto)
        {
            return ((Table)tblFormaPagto.Rows[(numPagto - 1) * 2 + 1].Cells[0].Controls[0]).Rows[0];
        }
    
        /// <summary>
        /// Retorna a função que altera a visibilidade dos itens do controle para cada pagamento.
        /// </summary>
        /// <param name="controle">A função será aplicada ao próprio controle de forma de pagamento?</param>
        /// <param name="numPagto">O número do pagamento do controle.</param>
        /// <param name="atualizarOpcoes">As opções devem ser atualizadas na execução da função?</param>
        /// <param name="desabilitarOpcoes">Desabilitar as opções dos controles?</param>
        /// <returns>Uma string com a função JavaScript que será usada para alterar os itens visíveis da forma de pagamento selecionada.</returns>
        private string GetFuncaoVisibilidade(bool controle, int numPagto, bool atualizarOpcoes, bool desabilitarOpcoes)
        {
            if (numPagto > 0)
            {
                string textoControle = controle ? "this" : "document.getElementById('" + this.ClientID + "_" + PrefixoControles(numPagto) + "drpFormaPagto')";
                return "alteraVisibilidade('" + tblFormaPagto.ClientID + "', " + numPagto + ", " + textoControle + ".value, " +
                    atualizarOpcoes.ToString().ToLower() + ", " + desabilitarOpcoes.ToString().ToLower() + ");";
            }
            else
            {
                string retorno = "atualizarOpcoes('" + tblFormaPagto.ClientID + "');";
                for (int i = 0; i < _numPossibilidadesPagto; i++)
                    retorno += "\n" + GetFuncaoVisibilidade(controle, i + 1, false, i == (_numPossibilidadesPagto - 1));
    
                return retorno;
            }
        }
    
        /// <summary>
        /// Retorna a função que altera a visibilidade dos itens do controle para cada pagamento.
        /// </summary>
        /// <param name="controle">A função será aplicada ao próprio controle de forma de pagamento?</param>
        /// <param name="numPagto">O número do pagamento do controle.</param>
        /// <returns>Uma string com a função JavaScript que será usada para alterar os itens visíveis da forma de pagamento selecionada.</returns>
        private string GetFuncaoVisibilidade(bool controle, int numPagto)
        {
            return GetFuncaoVisibilidade(controle, numPagto, true, true);
        }
    
        /// <summary>
        /// Formata algumas células do controle.
        /// </summary>
        /// <param name="celulas">As células que serão formatadas.</param>
        private void FormatarCelulas(params TableCell[] celulas)
        {
            foreach (TableCell celula in celulas)
            {
                celula.Width = new Unit("1px");
                celula.Wrap = false;
                celula.Style.Add("Padding-left", "2px");
                celula.Style.Add("Padding-right", "2px");
            }
        }
    
        /// <summary>
        /// Esconde algumas células do controle.
        /// </summary>
        /// <param name="celulas">As células que serão escondidas.</param>
        private void EsconderCelulas(params TableCell[] celulas)
        {
            foreach (TableCell celula in celulas)
                celula.Style.Add("Display", "none");
        }
    
        /// <summary>
        /// Formata um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
        private void FormatControl(Control campo)
        {
            // Garante que o campo seja válido
            if (campo == null || !(campo is WebControl))
                return;
    
            // String com o atributo que será alterado
            string atributo;
            if (campo is DropDownList)
                atributo = "OnChange";
            else if (campo is CheckBox)
                atributo = "OnClick";
            else
                atributo = "OnBlur";
    
            // String com a função que será executada
            string funcao = "";
    
            // Verifica se o controle já possui uma função atribuída ao evento OnBlur
            if (!String.IsNullOrEmpty(((WebControl)campo).Attributes[atributo]))
            {
                if ((((WebControl)campo).Attributes[atributo]).IndexOf(GetFuncaoCalculo()) > -1)
                    return;
                
                // Recupera a função do controle
                funcao = ((WebControl)campo).Attributes[atributo];
    
                // Verifica se a função desejada já está no controle
                if (funcao.IndexOf(GetFuncaoCalculo()) > -1)
                    return;
    
                // Coloca a função de cálculo junto à função original
                if (funcao.IndexOf("return") > -1)
                    funcao = funcao.Replace("return", GetFuncaoCalculo() + "; return");
                else
                    funcao += "; " + GetFuncaoCalculo();
            }
    
            // Indica que apenas essa função será executada
            else
                funcao = GetFuncaoCalculo();
    
            // Atribui a função ao controle
            if (((WebControl)campo).Attributes[atributo] == null || !((WebControl)campo).Attributes[atributo].Contains(funcao))
                ((WebControl)campo).Attributes[atributo] = funcao;
        }
    
        /// <summary>
        /// Retorna o ClientID de um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
        /// <returns>Uma string com o ClientID do controle.</returns>
        private string GetControlID(Control campo)
        {
            // Garante que o campo seja válido
            if (campo == null)
                return "";
    
            // Retorna o identificador do campo na página cliente
            return campo.ClientID;
        }
    
        /// <summary>
        /// Retorna o texto da função de cálculo do valor a ser pago.
        /// </summary>
        /// <returns>Uma string com a função executada no JavaScript.</returns>
        private string GetFuncaoCalculo()
        {
            return "usarCredito('" + this.ClientID + "', " + CallbackTotal + ", '" + CallbackUsarCredito + "')";
        }
    
        /// <summary>
        /// Retorna o tipo de um cheque a partir de uma string.
        /// </summary>
        /// <param name="tipo">A string com o tipo do cheque.</param>
        /// <returns></returns>
        private int GetTipo(string tipo)
        {
            switch (tipo.Trim().ToLower())
            {
                case "proprio":
                case "próprio":
                case "1":
                    return 1;
    
                default:
                    return 2;
            }
        }
    
        /// <summary>
        /// Retorna os dados da associação das contas bancárias.
        /// </summary>
        /// <returns></returns>
        private string GetAssocContaBanco()
        {
            // Variável de formato
            string formato = "" +
                "TipoCartao: {0}, " +
                "TipoBoleto: {1}, " +
                "ContaBanco: {2}, " +
                "Bloquear: {3}";
    
            // Variáveis de controle
            string retorno = "";
            var associacoes = AssocContaBancoDAO.Instance.GetByLoja(UserInfo.GetUserInfo.IdLoja);
    
            for (int i = 0; i < associacoes.Count; i++)
            {
                object[] dadosFormato = new object[4];
                dadosFormato[0] = associacoes[i].IdTipoCartao != null ? associacoes[i].IdTipoCartao.Value.ToString() : "null";
                dadosFormato[1] = associacoes[i].IdTipoBoleto != null ? associacoes[i].IdTipoBoleto.Value.ToString() : "null";
                dadosFormato[2] = Glass.Conversoes.StrParaUintNullable(associacoes[i].IdContaBanco.ToString()) != null ? associacoes[i].IdContaBanco.ToString() : "null";
                dadosFormato[3] = associacoes[i].BloquearContaBanco.ToString().ToLower();
                retorno += ", { " + String.Format(formato, dadosFormato) + " }";
            }
    
            // Retorna a variável para o JavaScript
            return "var assoc_conta_banco = new Array(" + (retorno.Length > 0 ? retorno.Substring(2) : "") + ");\n";
        }
    
        /// <summary>
        /// Retorna os dados dos juros dos cartões.
        /// </summary>
        /// <returns></returns>
        private string GetJurosCartao()
        {
            // Variável de formato
            string formato = "" +
                "TipoCartao: {0}, " +
                "Parcelas: {1}, " +
                "Juros: {2}";
    
            // Variáveis de controle
            string retorno = "";
    
            foreach (TipoCartaoCredito tc in TipoCartaoCreditoDAO.Instance.GetOrdered((int)Situacao.Ativo))
                for (int i = 0; i < tc.NumParc; i++)
                {
                    JurosParcelaCartao juros = JurosParcelaCartaoDAO.Instance.GetByTipoCartaoNumParc((uint)tc.IdTipoCartao, UserInfo.GetUserInfo.IdLoja, i + 1);
                    
                    object[] dadosFormato = new object[3];
                    dadosFormato[0] = tc.IdTipoCartao;
                    dadosFormato[1] = i + 1;
                    dadosFormato[2] = juros.Juros.ToString().Replace(",", ".");
                    retorno += ", { " + String.Format(formato, dadosFormato) + " }";
                }
    
            return "var juros_parc_cartao = new Array(" + (retorno.Length > 0 ? retorno.Substring(2) : "") + ");\n";
        }
    
        /// <summary>
        /// Retorna o valor que está em um controle.
        /// </summary>
        /// <param name="controle">O controle que será lido.</param>
        /// <returns>A propriedade que contém o valor do campo.</returns>
        private string GetValueFromControl(Control controle)
        {
            if (controle == null)
                return "";
    
            if (controle is TextBox)
                return ((TextBox)controle).Text;
            else if (controle is DropDownList)
                return ((DropDownList)controle).SelectedValue;
            else if (controle is HiddenField)
                return ((HiddenField)controle).Value;
    
            throw new ArgumentException("Tipo de controle '" + controle.GetType().Name + "' não implementado.");
        }
    
        #endregion
    
        #region Métodos AJAX
    
        /// <summary>
        /// Recupera os dados do cliente atual.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetDadosCliente(string idClienteStr)
        {
            string formato = @"
                ID: {0},
                Nome: '{1}',
                IsConsumidorFinal: {2}
            ";
    
            uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
            string nomeCliente = ClienteDAO.Instance.GetNome(idCliente);
    
            object[] dadosFormato = new object[] {
                idClienteStr,
                nomeCliente,
                ClienteDAO.Instance.IsConsumidorFinal(idCliente).ToString().ToLower()
            };
    
            return "{" + String.Format(formato, dadosFormato) + "}";
        }
    
        /// <summary>
        /// Retorna uma string com a forma de pagamento passada.
        /// </summary>
        /// <param name="formaPagto">O ID da forma de pagamento.</param>
        /// <returns>Uma string com a descrição da forma de pagamento.</returns>
        [Ajax.AjaxMethod]
        public string GetFormaPagto(string formaPagto)
        {
            return MetodosAjax.GetFormaPagto(formaPagto).ToLower();
        }
    
        /// <summary>
        /// Retorna uma string separada por "|" com as formas de pagamento passadas.
        /// </summary>
        /// <param name="formasPagto">Os IDs das formas de pagamento, separadas por "|".</param>
        /// <returns>Uma string separada por "|" com as descrições das formas de pagamento.</returns>
        [Ajax.AjaxMethod]
        public string GetFormasPagto(string formasPagto)
        {
            string[] formaPagto = formasPagto.Split('|');
            string retorno = "";
    
            foreach (string s in formaPagto)
                retorno += "|" + GetFormaPagto(s);
    
            return retorno.Length > 0 ? retorno.Substring(1) : "";
        }
    
        /// <summary>
        /// Retorna uma string separada por "|" com as formas de pagamento para um cliente.
        /// </summary>
        /// <param name="idClienteStr">O ID do cliente.</param>
        /// <returns>Uma string separada por "|" com os dados das formas de pagamento do cliente.</returns>
        [Ajax.AjaxMethod]
        public string GetFormasPagtoCliente(string idClienteStr, string idsFormasPagtoStr)
        {
            uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
            string retorno = "";
            var formaPagto = idCliente > 0 ? FormaPagtoDAO.Instance.GetByCliente(idCliente) : FormaPagtoDAO.Instance.GetForQuitarChequeDev();
    
            var idsFormasPagto = new List<string>(idsFormasPagtoStr.Split(','));

            foreach (var f in formaPagto.OrderBy(f => f.Descricao))
            {
                if (f.IdFormaPagto != null && idsFormasPagto.Contains(f.IdFormaPagto.ToString()))
                    retorno += "|" + f.IdFormaPagto + ";" + f.Descricao;
    
                if (f.IdFormaPagto == (uint)Pagto.FormaPagto.Deposito && idsFormasPagto.Contains(((uint)Pagto.FormaPagto.DepositoNaoIdentificado).ToString()))
                    retorno += "|" + (uint)Pagto.FormaPagto.DepositoNaoIdentificado + ";Depósito Não Identificado";

                if (f.IdFormaPagto == (uint)Pagto.FormaPagto.Cartao && idsFormasPagto.Contains(((uint)Pagto.FormaPagto.CartaoNaoIdentificado).ToString()))
                    retorno += "|" + (uint)Pagto.FormaPagto.CartaoNaoIdentificado + ";Cartão Não Identificado";
            }
    
            return retorno.Length > 0 ? retorno.Substring(1) : "";
        }
    
        /// <summary>
        /// Retorna uma string com o valor da comissão do comissionado.
        /// </summary>
        /// <param name="ids">Os IDs das contas que serão verificadas.</param>
        /// <param name="tipoModel">O tipo da model das contas.</param>
        /// <returns>Uma string com o valor.</returns>
        [Ajax.AjaxMethod]
        public string GetValorComissao(string ids, string tipo)
        {
            return UtilsFinanceiro.GetValorComissao(ids, tipo).ToString();
        }
    
        /// <summary>
        /// Retorna o número de parcelas para um cartão de crédito.
        /// </summary>
        /// <param name="tipoCartao">O tipo do cartão de crédito</param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetNumeroParcelas(string tipoCartao)
        {
            uint idTipoCartao = Conversoes.StrParaUint(tipoCartao);
            return TipoCartaoCreditoDAO.Instance.GetNumParcelas(idTipoCartao).ToString();
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (ExibirApenasCartaoDebito)
                odsTipoCartao.SelectParameters["tipo"].DefaultValue = "1";

            // Registra os scripts para o controle apenas uma vez
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlFormaPagto"))
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(ctrlFormaPagto));
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlFormaPagto", this.ResolveClientUrl("~/Scripts/ctrlFormaPagto.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlFormaPagto_Cheque", this.ResolveClientUrl("~/Scripts/Cheque.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlFormaPagto_CNI", this.ResolveClientUrl("~/Scripts/CNI.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlFormaPagto_AssocContaBanco", GetAssocContaBanco(), true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlFormaPagto_JurosCartao", GetJurosCartao(), true);
            }
    
            // Se for utilizado nas telas de pagamento altera os Labels
            if (!_isRecebimento)
            {
                lblDataRecebimento.Text = "Data Pagto.";
                chkRecebimentoParcial.Text = "Pagto. Parcial";
                lblInicioCredito.Text = "A empresa possui";
                lblFimCredito.Text = "de Crédito com esse Fornecedor";
            }
    
            // Criado para o recebimento de sinal
            // Força que o campo de crédito seja recalculado para o valor máximo
            if (_recalcularCredito)
                txtCreditoUtilizado.Text = null;
    
            // Define o valor dos HiddenField de exibição
            hdfExibirCredito.Value = ExibirCredito.ToString();
    
            // A alteração da data só fica disponível se a empresa trabalha com movimentação retroativa
            imgDataRecebido.Visible = true;
    
            // Adiciona o evento OnBlur ao campo de txtNumCli
            txtNumCli.Attributes.Add("OnBlur", "getCliFormaPagto('" + this.ClientID + "', this, document.getElementById('" + txtNomeCliente.ClientID + "'))");
    
            // Adiciona o evento OnChange ao campo de txtJuros
            txtJuros.Attributes.Add("OnChange", "alteraJuros('" + this.ClientID + "', true, " + CallbackTotal + ", '" + CallbackUsarCredito + "')");
    
            // Apenas exibe o CheckBox chkGerarCredito se o config estiver marcado para exibir
            chkGerarCredito.Visible = true;
    
            // Adiciona o callback ao CheckBox chkGerarCredito
            chkGerarCredito.Attributes.Add("OnClick", "gerarCredito('" + this.ClientID + "', '" + CallbackGerarCredito + "')");
    
            // Adiciona o evento OnClick para o CheckBox chkRecebimentoParcial
            chkRecebimentoParcial.Attributes.Add("OnClick", "recebimentoParcial('" + this.ClientID + "', '" + 
                FuncaoDadosRecebParcial + "', '" + CallbackRecebimentoParcial + "')");
    
            // Adiciona o evento OnClick para o CheckBox chkUsarCredito
            chkUsarCredito.Attributes.Add("OnClick", GetFuncaoCalculo());
    
            // Adiciona o evento OnClick para o CheckBox chkComissaoComissionado
            chkComissaoComissionado.Attributes.Add("OnClick", GetFuncaoCalculo());
    
            // Adiciona o evento OnChange para o CheckBox chkComissaoComissionado
            txtCreditoUtilizado.Attributes.Add("OnChange", GetFuncaoCalculo());
    
            // Indica o evento de PreRender da página
            this.PreRender += new EventHandler(ctrlFormaPagto_PreRender);
        }
    
        protected void tblFormaPagto_Load(object sender, EventArgs e)
        {
            // Cria um controle de forma de pagamento para cada possibilidade
            for (int i = 0; i < _numPossibilidadesPagto; i++)
            {
                // Cria o prefixo dos controles
                string prefixo = PrefixoControles(i + 1);
    
                // Cria as linhas e as adiciona à tabela
                TableRow linhaCima = new TableRow();
                linhaCima.ID = prefixo + "LinhaCima";
                TableRow linhaBaixo = new TableRow();
                linhaBaixo.ID = prefixo + "LinhaBaixo";
                linhaBaixo.Style.Add("Display", "None");
                tblFormaPagto.Rows.AddRange(new TableRow[] { linhaCima, linhaBaixo });
    
                #region Adiciona os estilos às linhas
    
                if (i % 2 == 0)
                {
                    if (!String.IsNullOrEmpty(_cssStyleLinha))
                    {
                        linhaCima.CssClass = _cssStyleLinha;
                        linhaBaixo.CssClass = _cssStyleLinha;
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(_cssStyleLinhaAlternada))
                    {
                        linhaCima.CssClass = _cssStyleLinhaAlternada;
                        linhaBaixo.CssClass = _cssStyleLinhaAlternada;
                    }
                }
    
                #endregion
    
                #region Cria as tabelas internas às linhas e aponta as variáveis de linha para as linhas dessas tabelas
    
                TableCell celulaCima = new TableCell();
                Table tabelaCima = new Table();
                tabelaCima.CellPadding = 0;
                tabelaCima.CellSpacing = 0;
                tabelaCima.Rows.Add(new TableRow());
                celulaCima.Controls.Add(tabelaCima);
                linhaCima.Controls.Add(celulaCima);
                linhaCima = tabelaCima.Rows[0];
                TableCell celulaBaixo = new TableCell();
                Table tabelaBaixo = new Table();
                tabelaBaixo.CellPadding = 0;
                tabelaBaixo.CellSpacing = 0;
                tabelaBaixo.Rows.Add(new TableRow());
                celulaBaixo.Controls.Add(tabelaBaixo);
                linhaBaixo.Controls.Add(celulaBaixo);
                linhaBaixo = tabelaBaixo.Rows[0];
    
                #endregion
    
                #region Cria as colunas e as adiciona à linha de cima
    
                TableCell valorTitulo = new TableCell();
                TableCell valorControles = new TableCell();
                TableCell formaPagtoTitulo = new TableCell();
                TableCell formaPagtoControles = new TableCell();
                TableCell cartaoTitulo = new TableCell();
                TableCell cartaoControles = new TableCell();
                TableCell boletoTipoTitulo = new TableCell();
                TableCell boletoTipoControles = new TableCell();
                TableCell cheques = new TableCell();
                TableCell cartoesNaoIdentificados = new TableCell();
    
                linhaCima.Cells.AddRange(new TableCell[] { valorTitulo, valorControles, formaPagtoTitulo, formaPagtoControles, cartaoTitulo, 
                    cartaoControles, boletoTipoTitulo, boletoTipoControles, cheques, cartoesNaoIdentificados });
    
                FormatarCelulas(valorTitulo, valorControles, formaPagtoTitulo, formaPagtoControles, cartaoTitulo, cartaoControles, boletoTipoTitulo, 
                    boletoTipoControles, cheques, cartoesNaoIdentificados);
    
                #endregion
    
                #region Cria as colunas e as adiciona à linha de baixo
    
                TableCell contaTitulo = new TableCell();
                TableCell contaControles = new TableCell();
                TableCell boletoTaxaTitulo = new TableCell();
                TableCell boletoTaxaControles = new TableCell();
                TableCell boletoNumeroTitulo = new TableCell();
                TableCell boletoNumeroControles = new TableCell();
                TableCell numAutTitulo = new TableCell();
                TableCell numAutControles = new TableCell();
                TableCell depositoNaoIdentTitulo = new TableCell();
                TableCell depositoNaoIdentControles = new TableCell();             
                TableCell antecipacaoFornecTitulo = new TableCell();
                TableCell antecipacaoFornecControles = new TableCell();
    
                linhaBaixo.Cells.AddRange(new TableCell[] { contaTitulo, contaControles, boletoTaxaTitulo, boletoTaxaControles, numAutTitulo, 
                    numAutControles, boletoNumeroTitulo, boletoNumeroControles });
    
                if (_isRecebimento)
                    linhaBaixo.Cells.AddRange(new TableCell[] { depositoNaoIdentTitulo, depositoNaoIdentControles });
                else
                    linhaBaixo.Cells.AddRange(new TableCell[] { antecipacaoFornecTitulo, antecipacaoFornecControles });
    
                FormatarCelulas(contaTitulo, contaControles, boletoTaxaTitulo, boletoTaxaControles, numAutTitulo, numAutControles, boletoNumeroTitulo, 
                    boletoNumeroControles, depositoNaoIdentTitulo, depositoNaoIdentControles, antecipacaoFornecTitulo, antecipacaoFornecControles);
    
                #endregion
    
                // Esconde as células ao abrir
                EsconderCelulas(cartaoTitulo, cartaoControles, boletoTipoTitulo, boletoTipoControles, cheques, cartoesNaoIdentificados, 
                    contaTitulo, contaControles, boletoTaxaTitulo, boletoTaxaControles, numAutTitulo, numAutControles, boletoNumeroTitulo, boletoNumeroControles);
    
                // Coloca os IDs nas células
                cartaoTitulo.ID = prefixo + "Cartao_Titulo";
                cartaoControles.ID = prefixo + "Cartao_Controles";
                boletoTipoTitulo.ID = prefixo + "BoletoTipo_Titulo";
                boletoTipoControles.ID = prefixo + "BoletoTipo_Controles";
                cheques.ID = prefixo + "Cheque";
                cartoesNaoIdentificados.ID = prefixo + "CartaoNaoIdentificado";
                contaTitulo.ID = prefixo + "Conta_Titulo";
                contaControles.ID = prefixo + "Conta_Controles";
                boletoTaxaTitulo.ID = prefixo + "BoletoTaxa_Titulo";
                boletoTaxaControles.ID = prefixo + "BoletoTaxa_Controles";
                boletoNumeroTitulo.ID = prefixo + "BoletoNumero_Titulo";
                boletoNumeroControles.ID = prefixo + "BoletoNumero_Controles";
                numAutTitulo.ID = prefixo + "NumAut_Titulo";
                numAutControles.ID = prefixo + "NumAut_Controles";
                depositoNaoIdentTitulo.ID = prefixo + "DepositoNaoIdentificado_Titulo";
                depositoNaoIdentControles.ID = prefixo + "DepositoNaoIdentificado_Controles";             
                antecipacaoFornecTitulo.ID = prefixo + "AntecipacaoFornecedor_Titulo";
                antecipacaoFornecControles.ID = prefixo + "AntecipacaoFornecedor_Controles";
    
                #region Cria os controles de valor
    
                valorTitulo.Text = _textoValorReceb + " " + (i + 1) + ":";
    
                TextBox txtValor = new TextBox();
                txtValor.ID = prefixo + "txtValor";
                txtValor.Width = new Unit("70px");
                txtValor.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true)");
                txtValor.Attributes.Add("OnChange", "recebimentoParcial('" + this.ClientID + "', '" +
                    FuncaoDadosRecebParcial + "', '" + CallbackRecebimentoParcial + "'); atualizaValorPago('" + this.ClientID + "', " + CallbackTotal + ")");
                txtValor.EnableViewState = this.EnableViewState;
                valorControles.Controls.Add(txtValor);
    
                CustomValidator ctvValor = new CustomValidator();
                ctvValor.ID = prefixo + "ctvValor";
                ctvValor.Text = " *";
                ctvValor.ErrorMessage = (i + 1) + "ª forma de pagamento: Valor não pode ser vazio.";
                ctvValor.ControlToValidate = txtValor.ID;
                ctvValor.ClientValidationFunction = "validaValorFormaPagto";
                ctvValor.ValidateEmptyText = true;
                ctvValor.Display = ValidatorDisplay.Dynamic;
                ctvValor.EnableViewState = this.EnableViewState;
                valorControles.Controls.Add(ctvValor);
                Page.Validators.Add(ctvValor);
    
                #endregion
    
                #region Cria os controles de forma de pagamento
    
                formaPagtoTitulo.Text = "Forma Pagto:";
    
                DropDownList drpFormaPagto = new DropDownList();
                drpFormaPagto.ID = prefixo + "drpFormaPagto";
                drpFormaPagto.DataSource = odsFormaPagto;
                drpFormaPagto.DataTextField = "Descricao";
                drpFormaPagto.DataValueField = "IdFormaPagto";
                drpFormaPagto.DataBind();
                drpFormaPagto.Attributes.Add("OnChange", GetFuncaoVisibilidade(true, i + 1) + "; " + GetFuncaoCalculo());
                drpFormaPagto.EnableViewState = this.EnableViewState;
                formaPagtoControles.Controls.Add(drpFormaPagto);
    
                CustomValidator ctvFormaPagto = new CustomValidator();
                ctvFormaPagto.ID = prefixo + "ctvFormaPagto";
                ctvFormaPagto.Text = " *";
                ctvFormaPagto.ErrorMessage = (i + 1) + "ª forma de pagamento: Forma de pagamento não pode ser vazia.";
                ctvFormaPagto.ControlToValidate = drpFormaPagto.ID;
                ctvFormaPagto.ClientValidationFunction = "validaValorFormaPagto";
                ctvFormaPagto.ValidateEmptyText = true;
                ctvFormaPagto.Display = ValidatorDisplay.Dynamic;
                ctvValor.EnableViewState = this.EnableViewState;
                formaPagtoControles.Controls.Add(ctvFormaPagto);
                Page.Validators.Add(ctvFormaPagto);
    
                #endregion
    
                #region Cria os controles de cartão
    
                cartaoTitulo.Text = "Cartão:";
    
                DropDownList drpTipoCartao = new DropDownList();
                drpTipoCartao.ID = prefixo + "drpTipoCartao";
                drpTipoCartao.Items.Add(new ListItem());
                drpTipoCartao.AppendDataBoundItems = true;
                drpTipoCartao.DataSource = odsTipoCartao;
                drpTipoCartao.DataTextField = "Descricao";
                drpTipoCartao.DataValueField = "IdTipoCartao";

                if (EfetuarBindContaBanco)
                    drpTipoCartao.DataBind();

                drpTipoCartao.Attributes.Add("OnChange", "getNumeroParcelas('" + this.ClientID + "', '" + prefixo + "', this.value); " + GetFuncaoVisibilidade(false, i + 1) + "; " + GetFuncaoCalculo());
                drpTipoCartao.EnableViewState = this.EnableViewState;
                cartaoControles.Controls.Add(drpTipoCartao);
                
                DropDownList drpParcCredito = new DropDownList();
                drpParcCredito.ID = prefixo + "drpParcCredito";
                drpParcCredito.Style.Add("Margin-left", "3px");
                drpParcCredito.Style.Add("Display", "none");
                for (int j = 1; j <= TipoCartaoCreditoDAO.Instance.GetMaxNumParcelas(); j++)
                    drpParcCredito.Items.Add(new ListItem(j + "x", j.ToString()));
                drpParcCredito.Attributes.Add("OnChange", GetFuncaoCalculo());
                drpParcCredito.EnableViewState = this.EnableViewState;
                cartaoControles.Controls.Add(drpParcCredito);

                if (!FinanceiroConfig.UtilizarTefCappta)
                {
                    Label lblNumAutCartao = new Label();
                    lblNumAutCartao.ID = prefixo + "lblNumAutCartao";
                    lblNumAutCartao.Text = "Nº Autorização:";
                    cartaoControles.Controls.Add(lblNumAutCartao);

                    TextBox txtNumAutCartao = new TextBox();
                    txtNumAutCartao.ID = prefixo + "txtNumAutCartao";
                    txtNumAutCartao.MaxLength = 30;
                    txtNumAutCartao.EnableViewState = this.EnableViewState;
                    cartaoControles.Controls.Add(txtNumAutCartao);

                    CustomValidator ctvNumAutCartao = new CustomValidator();
                    ctvNumAutCartao.ID = prefixo + "ctvNumAutCartao";
                    ctvNumAutCartao.Text = " *";
                    ctvNumAutCartao.ControlToValidate = txtNumAutCartao.ID;
                    ctvNumAutCartao.ErrorMessage = (i + 1) + "ª forma de pagamento: O número de autorização não pode ser vazio.";
                    ctvNumAutCartao.ClientValidationFunction = "validaNumAutCartao";
                    ctvNumAutCartao.ValidateEmptyText = true;
                    ctvNumAutCartao.Display = ValidatorDisplay.Dynamic;
                    ctvNumAutCartao.EnableViewState = this.EnableViewState;
                    cartaoControles.Controls.Add(ctvNumAutCartao);
                    Page.Validators.Add(ctvNumAutCartao);
                }

                if (FinanceiroConfig.Cartao.CobrarJurosCartaoCliente && _cobrarJurosCartaoClientes)
                {
                    Label lblDadosPagtoCartao = new Label();
                    lblDadosPagtoCartao.ID = prefixo + "lblDadosPagtoCartao";
                    lblDadosPagtoCartao.ForeColor = System.Drawing.Color.Blue;
                    lblDadosPagtoCartao.Style.Value = "display: inline-table; position: relative; top: -8px; left: 6px; margin-bottom: -8px";
    
                    Label lblValorCartao = new Label();
                    lblValorCartao.ID = lblDadosPagtoCartao.ID + "_lblValorCartao";
                    lblValorCartao.Text = "Valor pago: R$ 0,00";
    
                    Label lblSeparadorCartao1 = new Label();
                    lblSeparadorCartao1.ID = lblDadosPagtoCartao.ID + "_lblSeparadorCartao1";
                    lblSeparadorCartao1.Text = "<br />";
    
                    Label lblJurosCartao = new Label();
                    lblJurosCartao.ID = lblDadosPagtoCartao.ID + "_lblJurosCartao";
                    lblJurosCartao.Text = "Juros: R$ 0,00";
    
                    Label lblSeparadorCartao2 = new Label();
                    lblSeparadorCartao2.ID = lblDadosPagtoCartao.ID + "_lblSeparadorCartao2";
                    lblSeparadorCartao2.Text = "&nbsp;&nbsp;";
    
                    Label lblValorEsperadoCartao = new Label();
                    lblValorEsperadoCartao.ID = lblDadosPagtoCartao.ID + "_lblValorEsperadoCartao";
                    lblValorEsperadoCartao.ForeColor = System.Drawing.Color.Green;
                    lblValorEsperadoCartao.Text = "Valor esperado: R$ 0,00";
                    
                    lblDadosPagtoCartao.Controls.Add(lblValorCartao);
                    lblDadosPagtoCartao.Controls.Add(lblSeparadorCartao1);
                    lblDadosPagtoCartao.Controls.Add(lblJurosCartao);
                    lblDadosPagtoCartao.Controls.Add(lblSeparadorCartao2);
                    lblDadosPagtoCartao.Controls.Add(lblValorEsperadoCartao);
                    cartaoControles.Controls.Add(lblDadosPagtoCartao);
                }
    
                CustomValidator ctvCartao = new CustomValidator();
                ctvCartao.ID = prefixo + "ctvCartao";
                ctvCartao.Text = " *";
                ctvCartao.ErrorMessage = (i + 1) + "ª forma de pagamento: Cartão deve ser selecionado.";
                ctvCartao.ControlToValidate = drpTipoCartao.ID;
                ctvCartao.ClientValidationFunction = "validaCartao";
                ctvCartao.ValidateEmptyText = true;
                ctvCartao.Display = ValidatorDisplay.Dynamic;
                ctvCartao.EnableViewState = this.EnableViewState;
                cartaoControles.Controls.Add(ctvCartao);
                Page.Validators.Add(ctvCartao);
    
                #endregion
    
                #region Cria os controles do boleto
    
                if (_exibirNumBoleto)
                {
                    boletoNumeroTitulo.Text = "Número do boleto:";
    
                    TextBox txtNumeroBoleto = new TextBox();
                    txtNumeroBoleto.ID = prefixo + "txtNumeroBoleto";
                    txtNumeroBoleto.EnableViewState = this.EnableViewState;
                    boletoNumeroControles.Controls.Add(txtNumeroBoleto);
                }
    
                if (_exibirTiposBoleto)
                {
                    boletoTipoTitulo.Text = "Tipo do boleto:";
    
                    DropDownList drpTipoBoleto = new DropDownList();
                    drpTipoBoleto.ID = prefixo + "drpTipoBoleto";
                    drpTipoBoleto.Items.Add(new ListItem());
                    drpTipoBoleto.AppendDataBoundItems = true;
                    drpTipoBoleto.DataSource = odsTipoBoleto;
                    drpTipoBoleto.DataTextField = "Descricao";
                    drpTipoBoleto.DataValueField = "IdTipoBoleto";
                    drpTipoBoleto.Attributes.Add("OnChange", GetFuncaoVisibilidade(true, i + 1));
                    drpTipoBoleto.DataBind();
                    drpTipoBoleto.EnableViewState = this.EnableViewState;
                    boletoTipoControles.Controls.Add(drpTipoBoleto);
    
                    CustomValidator ctvBoleto = new CustomValidator();
                    ctvBoleto.ID = prefixo + "ctvBoleto";
                    ctvBoleto.Text = " *";
                    ctvBoleto.ErrorMessage = (i + 1) + "ª forma de pagamento: Tipo de boleto deve ser selecionado.";
                    ctvBoleto.ControlToValidate = drpTipoBoleto.ID;
                    ctvBoleto.ClientValidationFunction = "validaBoleto";
                    ctvBoleto.ValidateEmptyText = true;
                    ctvBoleto.Display = ValidatorDisplay.Dynamic;
                    ctvBoleto.EnableViewState = this.EnableViewState;
                    boletoTipoControles.Controls.Add(ctvBoleto);
                    Page.Validators.Add(ctvBoleto);
    
                    boletoTaxaTitulo.Text = "Taxa de antecipação:";
    
                    TextBox txtTaxaAntecipacao = new TextBox();
                    txtTaxaAntecipacao.Width = new Unit("70px");
                    txtTaxaAntecipacao.ID = prefixo + "txtTaxaAntecipacao";
                    txtTaxaAntecipacao.EnableViewState = this.EnableViewState;
                    txtTaxaAntecipacao.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true);");
                    boletoTaxaControles.Controls.Add(txtTaxaAntecipacao);
                }

                #endregion

                #region Cria os controles de cartão não identificado

                LinkButton lnkCartaoNaoIdentificado = new LinkButton();
                lnkCartaoNaoIdentificado.ID = prefixo + "lnkCartaoNaoIdentificado";
                lnkCartaoNaoIdentificado.OnClientClick = "openWindowCNI('"+ this.ClientID + "_TabelaCNI', " + "'" + tblFormaPagto.ClientID + "', " + (i + 1) + "); return false";
                lnkCartaoNaoIdentificado.Text = "Cartão Não Identificado";
                lnkCartaoNaoIdentificado.EnableViewState = false;
                lnkCartaoNaoIdentificado.CausesValidation = false;
                lnkCartaoNaoIdentificado.Attributes.Add("href", "");
                cartoesNaoIdentificados.Controls.Add(lnkCartaoNaoIdentificado);

                CustomValidator ctvCNI = new CustomValidator();
                ctvCNI.ID = prefixo + "ctvCNI";
                ctvCNI.Text = " *";
                ctvCNI.ErrorMessage = (i + 1) + "ª forma de pagamento: Cadastre pelo menos 1 cartão não identificado para prosseguir.";
                ctvCNI.ValidateEmptyText = true;
                ctvCNI.Display = ValidatorDisplay.Dynamic;
                ctvCNI.EnableViewState = this.EnableViewState;
                cartoesNaoIdentificados.Controls.Add(ctvCNI);
                Page.Validators.Add(ctvCNI);

                #endregion  

                #region Cria os controles de cheque

                LinkButton lnkCheque = new LinkButton();
                lnkCheque.ID = prefixo + "lnkCheque";
                lnkCheque.OnClientClick = "openWindowCheques('" + this.ClientID + "', '" + tblFormaPagto.ClientID + "', " + (i + 1) + ", " + _selecionarCheque.ToString().ToLower() + ", " + 
                    _cadastrarCheque.ToString().ToLower() + ", " + FuncaoQueryStringCheques + (_selecionarCheque ? " + '&tipo=4'" : "") + ", " + GetUrlCheques(i, true) + ", " +
                    CallbackIncluirCheque + ", " + CallbackExcluirCheque + ", document.getElementById('" + drpFormaPagto.ClientID + "').value); return false";
                lnkCheque.Text = "Cheques";
                lnkCheque.EnableViewState = false;
                lnkCheque.CausesValidation = false;
                lnkCheque.Attributes.Add("href", "");
                cheques.Controls.Add(lnkCheque);
                
                CustomValidator ctvCheque = new CustomValidator();
                ctvCheque.ID = prefixo + "ctvCheque";
                ctvCheque.Text = " *";
                ctvCheque.ErrorMessage = (i + 1) + "ª forma de pagamento: Cadastre pelo menos 1 cheque para prosseguir.";
                ctvCheque.ClientValidationFunction = "validaCheque";
                ctvCheque.ValidateEmptyText = true;
                ctvCheque.Display = ValidatorDisplay.Dynamic;
                ctvCheque.EnableViewState = this.EnableViewState;
                cheques.Controls.Add(ctvCheque);
                Page.Validators.Add(ctvCheque);
    
                if (FinanceiroConfig.FormaPagamento.NumeroDiasImpedirGerarCreditoCheque > 0)
                {
                    CustomValidator ctvDiasCheque = new CustomValidator();
                    ctvDiasCheque.ID = prefixo + "ctvDiasCheque";
                    ctvDiasCheque.Text = " *";
                    ctvDiasCheque.ErrorMessage = (i + 1) + "ª forma de pagamento: Não é possível gerar crédito para cheques com mais de " + FinanceiroConfig.FormaPagamento.NumeroDiasImpedirGerarCreditoCheque + " dias.";
                    ctvDiasCheque.ClientValidationFunction = "validaDiasCheque";
                    ctvDiasCheque.ValidateEmptyText = true;
                    ctvDiasCheque.Display = ValidatorDisplay.Dynamic;
                    ctvDiasCheque.EnableViewState = this.EnableViewState;
                    cheques.Controls.Add(ctvDiasCheque);
                    Page.Validators.Add(ctvDiasCheque);
                }
    
                #endregion
    
                #region Cria os controles de conta bancária
    
                contaTitulo.Text = "Conta Bancária:";
    
                DropDownList drpContaBanco = new DropDownList();
                drpContaBanco.ID = prefixo + "drpContaBanco";
                drpContaBanco.Items.Add(new ListItem());
                drpContaBanco.AppendDataBoundItems = true;
                drpContaBanco.DataSource = odsContaBanco;
                drpContaBanco.DataTextField = "Descricao";
                drpContaBanco.DataValueField = "IdContaBanco";
    
                if (EfetuarBindContaBanco)
                    drpContaBanco.DataBind();
    
                drpContaBanco.EnableViewState = this.EnableViewState;
                contaControles.Controls.Add(drpContaBanco);
                
                CustomValidator ctvConta = new CustomValidator();
                ctvConta.ID = prefixo + "ctvConta";
                ctvConta.Text = " *";
                ctvConta.ControlToValidate = drpContaBanco.ID;
                ctvConta.ErrorMessage = (i + 1) + "ª forma de pagamento: Selecione a conta bancária.";
                ctvConta.ClientValidationFunction = "validaConta";
                ctvConta.ValidateEmptyText = true;
                ctvConta.Display = ValidatorDisplay.Dynamic;
                ctvConta.EnableViewState = this.EnableViewState;
                contaControles.Controls.Add(ctvConta);
                Page.Validators.Add(ctvConta);
    
                #endregion
    
                #region Cria os controles de depósito não identificado
    
                if (_isRecebimento)
                {
                    depositoNaoIdentTitulo.Text = "Depósito não identificado:";
    
                    var odsDni = new Colosoft.WebControls.VirtualObjectDataSource();
                    odsDni.ID = prefixo + "odsDni";
                    odsDni.TypeName = "Glass.Data.DAL.DepositoNaoIdentificadoDAO";
                    odsDni.SelectMethod = "GetNaoUtilizados";
                    odsDni.EnableViewState = this.EnableViewState;
                    depositoNaoIdentControles.Controls.Add(odsDni);
    
                    Controls.ctrlSelPopup selDni = LoadControl("ctrlSelPopup.ascx") as Controls.ctrlSelPopup;
                    selDni.ID = prefixo + "selDni";
                    selDni.DataSourceID = odsDni.ID;
                    selDni.DataValueField = "IdDepositoNaoIdentificado";
                    selDni.DataTextField = "Descricao";
                    selDni.ExibirIdPopup = true;
                    selDni.PermitirVazio = false;
                    selDni.FazerPostBackBotaoPesquisar = false;
                    selDni.ColunasExibirPopup = "IdDepositoNaoIdentificado|DescrContaBanco|ValorMov|DataMov|Obs";
                    selDni.TextWidth = new Unit("300px");
                    selDni.TitulosColunas = "Cód.|Conta bancária|Valor depósito|Data depósito|Obs.";
                    selDni.EnableViewState = this.EnableViewState;
                    selDni.TituloTela = "Selecione o depósito não identificado";
                    selDni.Validador = new CustomValidator()
                    {
                        Text = " *",
                        ErrorMessage = (i + 1) + "ª forma de pagamento: Selecione o depósito não identificado.",
                        ClientValidationFunction = "validaDepositoNaoIdentificado",
                        EnableViewState = this.EnableViewState
                    };
                    selDni.CallbackSelecao = "selecaoDni";
                    depositoNaoIdentControles.Controls.Add(selDni);
                    Page.Validators.Add(selDni.Validador);
                }

                #endregion

                #region Cria os controles de antecipação de fornecedor

                if (!_isRecebimento)
                {
                    antecipacaoFornecTitulo.Text = "Antecipação de pagamento:";
    
                    var odsAntecipFornec = new Colosoft.WebControls.VirtualObjectDataSource();
                    odsAntecipFornec.ID = prefixo + "odsAntecipFornec";
                    odsAntecipFornec.TypeName = "Glass.Data.DAL.AntecipacaoFornecedorDAO";
                    odsAntecipFornec.SelectMethod = "ObtemAntecipacoesEmAberto";
                    odsAntecipFornec.EnableViewState = this.EnableViewState;
                    if (_campoIdFornecedor != null) odsAntecipFornec.SelectParameters.Add(new ControlParameter("idFornec", _campoIdFornecedor.ID));
                    antecipacaoFornecControles.Controls.Add(odsAntecipFornec);
    
                    Controls.ctrlSelPopup selAntecipFornec = LoadControl("ctrlSelPopup.ascx") as Controls.ctrlSelPopup;
                    selAntecipFornec.ID = prefixo + "selAntecipFornec";
                    selAntecipFornec.DataSourceID = odsAntecipFornec.ID;
                    selAntecipFornec.DataValueField = "IdAntecipFornec";
                    selAntecipFornec.DataTextField = "DescricaoControleFormaPagto";
                    selAntecipFornec.UsarValorRealControle = true;
                    selAntecipFornec.ExibirIdPopup = true;
                    selAntecipFornec.PermitirVazio = false;
                    selAntecipFornec.FazerPostBackBotaoPesquisar = false;
                    selAntecipFornec.ColunasExibirPopup = "IdAntecipFornec|Descricao|ValorAntecip|Saldo";
                    selAntecipFornec.TextWidth = new Unit("300px");
                    selAntecipFornec.TitulosColunas = "Cód.|Descrição|Valor do pagamento|Saldo atual";
                    selAntecipFornec.EnableViewState = this.EnableViewState;
                    selAntecipFornec.TituloTela = "Selecione a antecipação de pagamento";
                    selAntecipFornec.Validador = new CustomValidator()
                    {
                        Text = " *",
                        ErrorMessage = (i + 1) + "ª forma de pagamento: Selecione a antecipação de pagamento.",
                        ClientValidationFunction = "validaAntecipacaoFornecedor",
                        EnableViewState = this.EnableViewState
                    };
                    selAntecipFornec.CallbackSelecao = "selecaoAntecipFornec";
                    antecipacaoFornecControles.Controls.Add(selAntecipFornec);
                    Page.Validators.Add(selAntecipFornec.Validador);
                }
    
                #endregion
    
                #region Cria os controles do Construcard
    
                numAutTitulo.Text = "Nº Autorização:";
    
                TextBox txtNumAutConstrucard = new TextBox();
                txtNumAutConstrucard.ID = prefixo + "txtNumAutConstrucard";
                txtNumAutConstrucard.MaxLength = 30;
                txtNumAutConstrucard.EnableViewState = this.EnableViewState;
                numAutControles.Controls.Add(txtNumAutConstrucard);
                
                CustomValidator ctvNumAut = new CustomValidator();
                ctvNumAut.ID = prefixo + "ctvNumAut";
                ctvNumAut.Text = " *";
                ctvNumAut.ControlToValidate = txtNumAutConstrucard.ID;
                ctvNumAut.ErrorMessage = (i + 1) + "ª forma de pagamento: O número de autorização não pode ser vazio.";
                ctvNumAut.ClientValidationFunction = "validaNumAut";
                ctvNumAut.ValidateEmptyText = true;
                ctvNumAut.Display = ValidatorDisplay.Dynamic;
                ctvNumAut.EnableViewState = this.EnableViewState;
                numAutControles.Controls.Add(ctvNumAut);
                Page.Validators.Add(ctvNumAut);
    
                #endregion
            }
        }
    
        protected void ctrlFormaPagto_PreRender(object sender, EventArgs e)
        {
            // String de formato da variável
            string formato = @"
                CampoValorCredito: '{0}',
                CampoValorConta: '{1}',
                CampoValorDesconto: '{2}',
                CreditoUtilizado: {3},
                Cheques: {4},
                ParentID: '{5}',
                FormasPagamento: {6},
                Valores: {7},
                TiposCartao: {8},
                ParcelasCartao: {9},
                Juros: {10},
                DataRecebimento: {11},
                ContasBanco: {12},
                NumeroConstrucard: {13},
                NumeroPagamentos: {14},
                PagamentoCheque: {15},
                DescricaoFormasPagamento: {16},
                RecebimentoParcial: {17},
                UsarCredito: {18},
                GerarCredito: {19},
                Limpar: {20},
                TaxasAntecipacao: {21},
                TiposBoleto: {22},
                ExibirValorAPagar: {23},
                IDs: new Array(),
                TipoModel: '{24}',
                AdicionarID: {25},
                RemoverID: {26},
                LimparIDs: {27},
                DescontarComissao: {28},
                CampoTipoDesconto: '{29}',
                CampoValorAcrescimo: '{30}',
                CampoTipoAcrescimo: '{31}',
                ClienteID: {32},
                ExibirCliente: {33},
                AdicionarIDs: {34},
                AtualizarComissao: false,
                AlterarJurosMinimos: {35},
                IsWebglassLite: {36},
                ExibirParcelasCartao: {37},
                CobrarJurosCartaoCliente: {38},
                CampoClienteID: '{39}',
                ClienteID_Atual: {40},
                PermitirTodasGerarCredito: {41},
                Calcular: {42},
                CampoValorObra: '{43}',
                ValorObra: {44},
                ValorPagarAtual: {45},
                NumerosBoleto: {46},
                AtualizaFormasPagamento: {47},
                FormasPagamentoDisponiveis: '{48}',
                DatasFormasPagamento: {49},
                SituacaoGerarCredito: {50},
                Habilitar: {51},
                BloquearCamposContaVazia: {52},
                AlteraAtributosCheques: {53},
                ExibirApenasCredito: {54},
                ExibindoApenasCredito: {55},
                NumeroDiasImpedirGerarCreditoCheque: {56},
                CallbackSelCliente: {57},
                PermitirValorPagarNegativo: {58},
                DepositosNaoIdentificados: {59}, 
                FornecedorID: '{60}',
                IsRecebimento: {61}, 
                AntecipacoesFornecedores: {62},
                NumeroAutCartao: {63},
                CartoesNaoIdentificados: {64}";
    
            // Remove as quebras de linha e espaços desnecessários
            formato = formato.Replace("\n", "").Replace("\r", "").Trim();
            while (formato.Contains("  "))
                formato = formato.Replace("  ", " ");
    
            // Formata os controles, colocando a função de cálculo quando os campos forem alterados
            FormatControl(_campoCredito);
            FormatControl(_campoValorConta);
            FormatControl(_campoValorDesconto);
            FormatControl(_campoTipoDesconto);
            FormatControl(_campoValorAcrescimo);
            FormatControl(_campoTipoAcrescimo);
            FormatControl(_campoIdCliente);
            FormatControl(_campoIdFornecedor);
            FormatControl(_campoValorObra);
    
            // Cria a linha do script com a variável do controle
            object[] dadosFormato = new object[] {
                GetControlID(_campoCredito != null ? _campoCredito : hdfFormaPagtoCreditoCliente),          // 0
                GetControlID(_campoValorConta),
                GetControlID(_campoValorDesconto),
                "function() { return getCreditoUsado('" + this.ClientID + "') }",
                "function() { return getChequesString('" + this.ClientID + "_TabelaCheques', " + _selecionarCheque.ToString().ToLower() + ") }",
                !String.IsNullOrEmpty(_parentId) ? _parentId : "",                                          // 5
                "function(asString) { return getFormasPagto('" + this.ClientID + "', asString) }",
                "function(asString) { return getValores('" + this.ClientID + "', asString) }",
                "function(asString) { return getTiposCartao('" + this.ClientID + "', asString) }",
                "function(asString) { return getParcelasCartao('" + this.ClientID + "', asString) }",
                "function() { return getJuros('" + this.ClientID + "') }",                                  // 10
                "function() { return getDataRecebimento('" + this.ClientID + "') }",
                "function(asString) { return getContasBanco('" + this.ClientID + "', asString) }",
                 "function(asString) { return getNumConstrucard('" + this.ClientID + "', asString) }",
                _numPossibilidadesPagto,
                "new Array()",                                                                              // 15
                "new Array(" + _numPossibilidadesPagto  + ")",
                "function() { return getRecebimentoParcial('" + this.ClientID + "') }",
                "function() { return getUsarCredito('" + this.ClientID + "') }",
                "function() { return getGerarCredito('" + this.ClientID + "') }",
                "function() { limparControle('" + this.ClientID + "', '" + CallbackUsarCredito + "') }",    // 20
                "function(asString) { return getTaxasAntecipacao('" + this.ClientID + "', asString) }",
                "function(asString) { return getTiposBoleto('" + this.ClientID + "', asString) }",
                ExibirValorAPagar.ToString().ToLower(),
                TipoModel,
                "function(id) { adicionarIdComissao('" + this.ClientID + "', id, false) }",                 // 25
                "function(id) { removerIdComissao('" + this.ClientID + "', id) }",
                "function() { limparIdsComissao('" + this.ClientID + "') }",
                "function() { return getDescontarComissao('" + this.ClientID + "') }",
                GetControlID(_campoTipoDesconto),
                GetControlID(_campoValorAcrescimo),                                                         
                GetControlID(_campoTipoAcrescimo),
                "function() { return getIdCliente('" + this.ClientID + "'); }",
                ExibirCliente.ToString().ToLower(),
                "function(ids) { adicionarIdsComissao('" + this.ClientID + "', ids); }",
                "function(valor) { setJurosMin('" + this.ClientID + "', valor); }",                         
                "false",
                FinanceiroConfig.Cartao.PedidoJurosCartao.ToString().ToLower(),
                (FinanceiroConfig.Cartao.CobrarJurosCartaoCliente && _cobrarJurosCartaoClientes).ToString().ToLower(),
                GetControlID(_campoIdCliente),
                0,                                                                                          
                FinanceiroConfig.FormaPagamento.GerarCreditoFormasPagto.ToString().ToLower(),
                "function() { " + GetFuncaoCalculo() + " }",
                GetControlID(_campoValorObra),
                "function() { return getValorObra('" + this.ClientID + "'); }",
                0,                                                                                          
                "function(asString) { return getNumerosBoleto('" + this.ClientID + "', asString) }",
                "function() { " + GetFuncaoVisibilidade(false, 0) + " }",
                GetFormasPagtoDisponiveis(),
                "function(asString) { return getDatasFormaPagto('" + this.ClientID + "', asString) }",      
                chkGerarCredito.Checked.ToString().ToLower(),
                "function(habilitar) { return habilitarControleFP('" + this.ClientID + "', habilitar) }",
                _bloquearCamposContaVazia.ToString().ToLower(),
                "function(nomeAtributo, valor) { alteraAtributos('" + this.ClientID + "_TabelaCheques', nomeAtributo, valor) }",
                "function(exibir) { exibirApenasCredito('" + this.ClientID + "', exibir) }",                
                "false",
                FinanceiroConfig.FormaPagamento.NumeroDiasImpedirGerarCreditoCheque,
                CallbackSelCliente,
                _permitirValorPagarNegativo.ToString().ToLower(),                                           
                "function(asString) { return getDepositosNaoIdentificados('" + this.ClientID + "', asString) }",
                GetControlID(_campoIdFornecedor),
                _isRecebimento.ToString().ToLower(),
                "function(asString) { return getAntecipacoesFornecedores('" + this.ClientID + "', asString) }",                     
                "function(asString) { return getNumAutCartao('" + this.ClientID + "', asString) }",         
                "function() { return getCNI('" + this.ClientID + "_TabelaCNI" + "') }",
            };
    
            // Define o script da variável na tela
            string script = "var " + this.ClientID + " = { " + String.Format(formato, dadosFormato) + " };\n";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, script, true);
    
            // Define o script de inicialização do controle na tela
            script = GetFuncaoVisibilidade(false, 0) + "\n";
            script += GetFuncaoCalculo() + ";\n";
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_Load", script, true);
    
            // Define o script de OnSubmit na tela
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), this.ClientID + "_OnSubmit", "atualizarChequesECNI('" + this.ClientID + "');\n");
        }
    
        protected void lnkSelCliente_Load(object sender, EventArgs e)
        {
            lnkSelCliente.OnClientClick = "openWindow(590, 760, '../Utils/SelCliente.aspx?controleFormaPagto=" + this.ClientID + 
                "'); return false;";
        }
    }
}
