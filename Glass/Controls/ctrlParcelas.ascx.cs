using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlParcelas : BaseUserControl
    {
        #region Campos privados
    
        private int _numParcelasLinha = 5;
        private int _numParcelas;
        private TextBox[] _camposDatas;
        private TextBox[] _camposValores;
        private TextBox[] _camposJuros;
        private TextBox[] _camposAdicionais;
        private DropDownList[] _formasPagto;
        private string _parentId = "";
        private bool _exibirValores = true;
        private bool _readOnly = false;
        private bool _alterarData = true;
        private bool _calcularData = true;
        private bool _calcularJurosParcela = false;
        private bool _exibirCampoAdicional = false;
        private bool _recalcularParcelasApenasTrocaValor = false;
        private string _tituloCampoAdicional = "";
        private string _callbackAplicarJuros = null;
        private string _callbackTotal = null;
        private bool _isCompra = false;
        private bool _liberarPedido = PedidoConfig.LiberarPedido;
        private char _splitChar = ',';
        private byte _diasSomarDataVazia = 1;
    
        private Control _campoValorTotal;
        private Control _campoValorEntrada;
        private Control _campoParcelasVisiveis;
        private Control _campoExibirParcelas;
        private Control _campoCalcularParcelas;
        private Control _campoValorDescontoAtual;
        private Control _campoTipoDescontoAtual;
        private Control _campoValorDescontoAnterior;
        private Control _campoTipoDescontoAnterior;
        private Control _campoValorAcrescimoAtual;
        private Control _campoTipoAcrescimoAtual;
        private Control _campoValorAcrescimoAnterior;
        private Control _campoTipoAcrescimoAnterior;
        private Control _campoTextoParcelas;
        private Control _campoDataBase;
        private Control _campoTaxaJurosParcela;
        private Control _campoValorObra;
        private Control _campoFormaPagto;
        #endregion
    
        #region Propriedades
    
        public string CssClass
        {
            get { return tblParcelas.CssClass; }
            set { tblParcelas.CssClass = value; }
        }
    
        public byte DiasSomarDataVazia
        {
            get { return _diasSomarDataVazia; }
            set { _diasSomarDataVazia = value; }
        }
    
        /// <summary>
        /// O caractere usado para separar os dias das parcelas.
        /// </summary>
        public char CaractereSeparacaoDiasParcelas
        {
            get { return _splitChar; }
            set { _splitChar = value; }
        }
    
        /// <summary>
        /// O controle está sendo utilizado para compra?
        /// </summary>
        public bool IsCompra
        {
            get { return _isCompra; }
            set { _isCompra = value; }
        }
    
        /// <summary>
        /// Define que 
        /// </summary>
        public bool LiberarPedido
        {
            get { return _liberarPedido; }
            set { _liberarPedido = value; }
        }
    
        /// <summary>
        /// O controle deve exibir os campos de valores?
        /// </summary>
        public bool ExibirValores
        {
            get { return _exibirValores; }
            set { _exibirValores = value; }
        }
    
        /// <summary>
        /// Função de callback que deve ser executada ao calcular o valor total das parcelas.
        /// </summary>
        public string CallbackTotal
        {
            get { return !String.IsNullOrEmpty(_callbackTotal) ? "'" + _callbackTotal + "'" : "''"; }
            set { _callbackTotal = value; }
        }
    
        /// <summary>
        /// Função de callback que deve ser executada ao clicar no checkbox de aplicação de juro na parcela.
        /// </summary>
        public string CallbackAplicarJuros
        {
            get { return !String.IsNullOrEmpty(_callbackAplicarJuros) ? "'" + _callbackAplicarJuros + "'" : "''"; }
            set { _callbackAplicarJuros = value; }
        }
    
        /// <summary>
        /// Deverá ser exibido um campo adicional?
        /// </summary>
        public bool ExibirCampoAdicional
        {
            get { return _exibirCampoAdicional; }
            set { _exibirCampoAdicional = value; }
        }
    
        /// <summary>
        /// Título que será exibido no campo adicional.
        /// </summary>
        public string TituloCampoAdicional
        {
            get { return _tituloCampoAdicional; }
            set { _tituloCampoAdicional = value; }
        }
    
        /// <summary>
        /// As parcelas terão aplicação de juros individual?
        /// </summary>
        public bool CalcularJurosParcela
        {
            get { return _calcularJurosParcela; }
            set { _calcularJurosParcela = value; }
        }
    
        /// <summary>
        /// O controle será somente-leitura?
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }
    
        /// <summary>
        /// O campo de data pode ser alterado pelo usuário?
        /// </summary>
        public bool AlterarData
        {
            get { return _alterarData; }
            set { _alterarData = value; }
        }
    
        /// <summary>
        /// O ID do controle pai.
        /// </summary>
        public string ParentID
        {
            get { return _parentId; }
            set { _parentId = value; }
        }
    
        /// <summary>
        /// Número de parcelas que será exibido por linha na tabela.
        /// </summary>
        public int NumParcelasLinha
        {
            get { return _numParcelasLinha; }
            set { _numParcelasLinha = value; }
        }
    
        /// <summary>
        /// Número máximo de parcelas que serão geradas.
        /// </summary>
        public int NumParcelas
        {
            get { return _numParcelas > 0 ? _numParcelas : PedidoConfig.FormaPagamento.NumParcelasPedido; }
            set 
            {
                _numParcelas = Math.Max(value, PedidoConfig.FormaPagamento.NumParcelasPedido);
                _camposDatas = new TextBox[_numParcelas];
                _camposValores = new TextBox[_numParcelas];
                _camposAdicionais = new TextBox[_numParcelas];
                _formasPagto = new DropDownList[_numParcelas];
                _camposJuros = new TextBox[_numParcelas];
            }
        }
    
        /// <summary>
        /// Define se as parcelas serão recalculadas apenas se houver troca de valor 
        /// dos campos que influenciam o valor das parcelas
        /// </summary>
        public bool RecalcularParcelasApenasTrocaValor
        {
            get { return _recalcularParcelasApenasTrocaValor; }
            set { _recalcularParcelasApenasTrocaValor = true; }
        }
    
        /// <summary>
        /// As datas das parcelas.
        /// </summary>
        [Bindable(true)]
        public DateTime[] Datas
        {
            get
            {
                DateTime[] datas = new DateTime[NumParcelas];
                for (int i = 0; i < NumParcelas; i++)
                {
                    if (_camposDatas[i].Text != "")
                        datas[i] = DateTime.ParseExact(_camposDatas[i].Text, "dd/MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR"));
                    else
                        datas[i] = new DateTime();
                }
    
                return datas;
            }
            set
            {
                for (int i = 0; i < NumParcelas; i++)
                {
                    if (value.Length > i && value[i].Ticks != 0)
                        _camposDatas[i].Text = value[i].ToString("dd/MM/yyyy");
                    else
                        _camposDatas[i].Text = "";
                }
            }
        }
    
        /// <summary>
        /// Os valores das parcelas.
        /// </summary>
        [Bindable(true)]
        public decimal[] Valores
        {
            get
            {
                decimal[] valores = new decimal[NumParcelas];
                for (int i = 0; i < NumParcelas; i++)
                    if (!String.IsNullOrEmpty(_camposValores[i].Text))
                        valores[i] = Glass.Conversoes.StrParaDecimal(_camposValores[i].Text);
    
                return valores;
            }
            set
            {
                for (int i = 0; i < NumParcelas; i++)
                    if (value.Length > i)
                        _camposValores[i].Text = value[i].ToString();
            }
        }
    
        /// <summary>
        /// Os juros das parcelas.
        /// </summary>
        [Bindable(true)]
        public decimal[] JurosParcelas
        {
            get
            {
                decimal[] juros = new decimal[NumParcelas];
                for (int i = 0; i < NumParcelas; i++)
                    if (!String.IsNullOrEmpty(_camposJuros[i].Text))
                        juros[i] = Glass.Conversoes.StrParaDecimal(_camposJuros[i].Text);
    
                return juros;
            }
            set
            {
                for (int i = 0; i < NumParcelas; i++)
                    if (value.Length > i)
                        _camposJuros[i].Text = value[i].ToString();
            }
        }
    
        /// <summary>
        /// Os dados dos campos adicionais.
        /// </summary>
        [Bindable(true)]
        public string[] Adicionais
        {
            get
            {
                if (!_exibirCampoAdicional)
                    return new string[NumParcelas];
    
                string[] adicionais = new string[NumParcelas];
                for (int i = 0; i < NumParcelas; i++)
                    adicionais[i] = _camposAdicionais[i].Text;
    
                return adicionais;
            }
            set
            {
                if (!_exibirCampoAdicional)
                    return;
    
                for (int i = 0; i < NumParcelas; i++)
                {
                    if (value.Length > i)
                        _camposAdicionais[i].Text = value[i];
                    else
                        _camposAdicionais[i].Text = "";
                }
            }
        }
    
        /// <summary>
        /// Os dados dos campos adicionais.
        /// </summary>
        [Bindable(true)]
        public uint[] FormasPagamento
        {
            get
            {
                if (!_isCompra)
                    return new uint[NumParcelas];
    
                uint[] formasPagto = new uint[NumParcelas];
                for (int i = 0; i < NumParcelas; i++)
                    formasPagto[i] = Glass.Conversoes.StrParaUint(_formasPagto[i].SelectedValue);
    
                return formasPagto;
            }
            set
            {
                if (!_isCompra)
                    return;
    
                for (int i = 0; i < NumParcelas; i++)
                {
                    if (value.Length > i)
                    {
                        if (_formasPagto[i].Items.Count == 0)
                            _formasPagto[i].DataBind();
    
                        _formasPagto[i].SelectedIndex = _formasPagto[i].Items.IndexOf(_formasPagto[i].Items.FindByValue(value[i].ToString()));
                    }
                    else
                        _formasPagto[i].SelectedIndex = 0;
                }
            }
        }
    
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set
            {
                // Atualiza o ViewState do controle
                base.EnableViewState = value;
    
                // Atualiza os ViewStates dos controles internos
                for (int i = 0; i < tblParcelas.Rows.Count; i++)
                    for (int j = 0; j < tblParcelas.Rows[i].Cells.Count; j++)
                        foreach (Control c in tblParcelas.Rows[i].Cells[j].Controls)
                            c.EnableViewState = value;
            }
        }
    
        #endregion
    
        #region Propriedades de controles da página
    
        /// <summary>
        /// Campo que possui o valor total.
        /// </summary>
        public Control CampoValorTotal
        {
            get { return _campoValorTotal; }
            set { _campoValorTotal = value; }
        }
    
        /// <summary>
        /// Campo que possui o valor da entrada.
        /// </summary>
        public Control CampoValorEntrada
        {
            get { return _campoValorEntrada; }
            set { _campoValorEntrada = value; }
        }
    
        /// <summary>
        /// Campo que indica o número de parcelas visíveis.
        /// </summary>
        public Control CampoParcelasVisiveis
        {
            get { return _campoParcelasVisiveis; }
            set { _campoParcelasVisiveis = value; }
        }
    
        /// <summary>
        /// Campo que indica se as parcelas serão exibidas.
        /// </summary>
        public Control CampoExibirParcelas
        {
            get { return _campoExibirParcelas; }
            set { _campoExibirParcelas = value; }
        }
    
        /// <summary>
        /// Campo que indica se as parcelas serão calculadas.
        /// </summary>
        public Control CampoCalcularParcelas
        {
            get { return _campoCalcularParcelas; }
            set { _campoCalcularParcelas = value; }
        }
    
        /// <summary>
        /// Campo que possui o valor do desconto atual.
        /// </summary>
        public Control CampoValorDescontoAtual
        {
            get { return _campoValorDescontoAtual; }
            set { _campoValorDescontoAtual = value; }
        }
    
        /// <summary>
        /// Campo que possui o tipo do desconto atual.
        /// </summary>
        public Control CampoTipoDescontoAtual
        {
            get { return _campoTipoDescontoAtual; }
            set { _campoTipoDescontoAtual = value; }
        }
    
        /// <summary>
        /// Campo que possui o valor do desconto anterior.
        /// </summary>
        public Control CampoValorDescontoAnterior
        {
            get { return _campoValorDescontoAnterior; }
            set { _campoValorDescontoAnterior = value; }
        }
    
        /// <summary>
        /// Campo que possui o tipo do desconto anterior.
        /// </summary>
        public Control CampoTipoDescontoAnterior
        {
            get { return _campoTipoDescontoAnterior; }
            set { _campoTipoDescontoAnterior = value; }
        }
    
        /// <summary>
        /// Campo que possui o valor do acréscimo atual.
        /// </summary>
        public Control CampoValorAcrescimoAtual
        {
            get { return _campoValorAcrescimoAtual; }
            set { _campoValorAcrescimoAtual = value; }
        }
    
        /// <summary>
        /// Campo que possui o tipo do acréscimo atual.
        /// </summary>
        public Control CampoTipoAcrescimoAtual
        {
            get { return _campoTipoAcrescimoAtual; }
            set { _campoTipoAcrescimoAtual = value; }
        }
    
        /// <summary>
        /// Campo que possui o valor do acréscimo anterior.
        /// </summary>
        public Control CampoValorAcrescimoAnterior
        {
            get { return _campoValorAcrescimoAnterior; }
            set { _campoValorAcrescimoAnterior = value; }
        }
    
        /// <summary>
        /// Campo que possui o tipo do acréscimo anterior.
        /// </summary>
        public Control CampoTipoAcrescimoAnterior
        {
            get { return _campoTipoAcrescimoAnterior; }
            set { _campoTipoAcrescimoAnterior = value; }
        }
    
        /// <summary>
        /// Campo que possui o texto que descreve as parcelas.
        /// </summary>
        public Control CampoTextoParcelas
        {
            get { return _campoTextoParcelas; }
            set { _campoTextoParcelas = value; }
        }
    
        /// <summary>
        /// Campo que possui a data base para cálculo dos prazos das datas.
        /// </summary>
        public Control CampoDataBase
        {
            get { return _campoDataBase; }
            set { _campoDataBase = value; }
        }
    
        /// <summary>
        /// Campo que possui a taxa de juros que será cobrada por parcela.
        /// </summary>
        public Control CampoTaxaJurosParcela
        {
            get { return _campoTaxaJurosParcela; }
            set { _campoTaxaJurosParcela = value; }
        }
    
        /// <summary>
        /// Campo que indica o valor utilizado da obra.
        /// </summary>
        public Control CampoValorObra
        {
            get { return _campoValorObra; }
            set { _campoValorObra = value; }
        }
    
        /// <summary>
        /// Campo que possui a forma de pagamento principal.
        /// </summary>
        public Control CampoFormaPagto
        {
            get { return _campoFormaPagto; }
            set { _campoFormaPagto = value; }
        }
    
        public bool CalcularData
        {
            get { return _calcularData; }
            set { _calcularData = value; }
        }
    
        public bool ExibirJurosPorParcela { get; set; }
    
        #endregion
    
        #region Métodos de suporte
    
        /// <summary>
        /// Formata um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
        private void FormatControl(Control campo)
        {
            string funcao = GetFuncaoCalculo();
    
            if (RecalcularParcelasApenasTrocaValor)
                funcao = "calcParcelasLoad=true;" + funcao;
    
            FormatControl(campo, funcao);
        }
    
        /// <summary>
        /// Formata um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
        /// <param name="funcaoExecutar">A função que será executada.</param>
        private void FormatControl(Control campo, string funcaoExecutar)
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
                if ((((WebControl)campo).Attributes[atributo]).IndexOf(funcaoExecutar) > -1)
                    return;
    
                // Recupera a função do controle
                funcao = ((WebControl)campo).Attributes[atributo];
    
                // Verifica se a função desejada já está no controle
                if (funcao.IndexOf(funcaoExecutar) > -1)
                    return;
    
                // Coloca a função de cálculo junto à função original
                if (funcao.IndexOf("return") > -1)
                    funcao = funcao.Replace("return", funcaoExecutar + "; return");
                else
                    funcao += "; " + funcaoExecutar;
            }
    
            // Indica que apenas essa função será executada
            else
                funcao = funcaoExecutar;
    
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
        /// Retorna uma string com a função de cálculo do JavaScript.
        /// </summary>
        /// <returns>A função usada no JavaScript para calcular as parcelas.</returns>
        public string GetFuncaoCalculo()
        {
            return "Parc_visibilidadeParcelas('" + this.ClientID + "', " + CallbackTotal + ", '" + _splitChar.ToString() + "')";
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Registra os scripts para o controle apenas uma vez
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), "ctrlParcelas"))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), "ctrlParcelas", this.ResolveClientUrl("~/Scripts/ctrlParcelas.js"));
    
            // Indica o evento de PreRender da página
            this.PreRender += new EventHandler(ctrlParcelas_PreRender);
    
            if (_numParcelas == 0)
                NumParcelas = NumParcelas;
        }
    
        protected void tblParcelas_Load(object sender, EventArgs e)
        {
            // Variável que contém a linha da tabela
            TableRow linha = new TableRow();
            
            for (int i = 0; i < NumParcelas; i++)
            {
                // Verifica se há a necessidade de se criar outra linha na tabela
                if (i > 0 && i % NumParcelasLinha == 0)
                {
                    // Salva a linha na tabela e começa outra
                    tblParcelas.Rows.Add(linha);
                    linha = new TableRow();
                }
    
                #region Célula que será inserida na linha
    
                TableCell parcela = new TableCell();
                parcela.ID = "Parc" + (i + 1);
    
                // Cria a tabela que reprensenta a parcela
                int numLinhas = _calcularJurosParcela && _exibirCampoAdicional ? 6 :
                    _calcularJurosParcela || _exibirCampoAdicional ? 5 : 4;
    
                if (_isCompra)
                    numLinhas++;
    
                Table dadosParcela = new Table();
                
                TableRow[] linhasParcela = new TableRow[numLinhas];
                linhasParcela[0] = new TableRow();
                linhasParcela[1] = new TableRow();
                linhasParcela[2] = new TableRow();
                linhasParcela[3] = new TableRow();
    
                if (_exibirCampoAdicional)
                    linhasParcela[4] = new TableRow();
    
                if (_calcularJurosParcela)
                    linhasParcela[numLinhas - 1 - (_isCompra ? 1 : 0)] = new TableRow();
    
                if (_isCompra)
                    linhasParcela[numLinhas - 1] = new TableRow();
    
                for (int j = 0; j < numLinhas; j++)
                    linhasParcela[j].ID = parcela.ID + "_Linha" + (j + 1);
    
                if (!_exibirValores)
                    linhasParcela[1].Style.Add("display", "none");
    
                dadosParcela.Rows.AddRange(linhasParcela);
    
                #endregion
    
                #region Cria a célula do título da parcela
    
                TableCell celula = new TableCell();
                celula.ColumnSpan = 2;
                celula.Text = (i + 1) + "ª parcela";
                linhasParcela[0].Cells.Add(celula);
    
                #endregion
    
                #region Cria as células com o valor da parcela
    
                celula = new TableCell();
                if (_exibirValores)
                    celula.Text = "Valor:";
                linhasParcela[1].Cells.Add(celula);
    
                celula = new TableCell();
                TextBox campoValor = new TextBox();
                campoValor.ID = parcela.ID + "_txtValor";
                campoValor.Width = new Unit("80px");
                campoValor.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true)");
                if (i == 0)
                    campoValor.Attributes.Add("OnChange", "Parc_calculaDemaisParcelas('" + this.ClientID + "')");
                campoValor.EnableViewState = this.EnableViewState;
                campoValor.Enabled = !_readOnly;
                _camposValores[i] = campoValor;
    
                if (_exibirValores)
                {
                    celula.Controls.Add(campoValor);
    
                    CustomValidator validaValor = new CustomValidator();
                    validaValor.ID = parcela.ID + "_ctvValidaValor";
                    validaValor.ControlToValidate = campoValor.ID;
                    validaValor.ValidateEmptyText = true;
                    validaValor.ClientValidationFunction = "validaValor";
                    validaValor.Display = ValidatorDisplay.Dynamic;
                    validaValor.Text = " *";
                    validaValor.ErrorMessage = (i + 1) + "ª parcela: Valor não pode ser vazio.";
                    validaValor.EnableViewState = this.EnableViewState;
                    Page.Validators.Add(validaValor);
                    celula.Controls.Add(validaValor);
                }
                
                linhasParcela[1].Cells.Add(celula);
    
                #endregion
    
                #region Cria as células com campo de juros por parcela
    
                if (ExibirJurosPorParcela)
                {
                    celula = new TableCell();
                    if (_exibirValores)
                        celula.Text = "Juros:";
                    linhasParcela[3].Cells.Add(celula);
    
                    celula = new TableCell();
                    TextBox campoJuros = new TextBox();
                    campoJuros.ID = parcela.ID + "_txtJuros";
                    campoJuros.Width = new Unit("80px");
                    campoJuros.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true)");
                    campoJuros.EnableViewState = this.EnableViewState;
                    //campoJuros.Enabled = !_readOnly;
                    _camposJuros[i] = campoJuros;
    
                    if (_exibirValores)
                    {
                        celula.Controls.Add(campoJuros);
                    }
    
                    linhasParcela[3].Cells.Add(celula);
                }
                #endregion
    
                #region Cria as células com a data da parcela
    
                celula = new TableCell();
                celula.Text = "Data:";
                linhasParcela[2].Cells.Add(celula);
    
                celula = new TableCell();
                celula.Wrap = false;
                TextBox campoData = new TextBox();
                campoData.ID = parcela.ID + "_txtData";
                campoData.Attributes.Add("OnKeypress", "return mascara_data(event, this), soNumeros(event, true, true);");
                campoData.Attributes.Add("MaxLength", "10");
                campoData.Attributes.Add("OnDateChange", "Parc_atualizaDiasParcelas('" + this.ClientID + "')");
                campoData.Width = new Unit("70px");
                campoData.EnableViewState = this.EnableViewState;
                campoData.Enabled = !_readOnly && _alterarData;
                _camposDatas[i] = campoData;
    
                celula.Controls.Add(campoData);
    
                ImageButton selecionaData = new ImageButton();
                selecionaData.ID = parcela.ID + "_imgData";
                selecionaData.ImageUrl = "~/Images/calendario.gif";
                selecionaData.OnClientClick = "return SelecionaData('" + this.ID + "_" + campoData.ID + "', this)";
                selecionaData.Style.Add("padding-left", "3px");
                selecionaData.Style.Add("margin-bottom", "-3px");
                selecionaData.ToolTip = "Selecionar";
                selecionaData.EnableViewState = false;
    
                if (!campoData.Enabled)
                    selecionaData.Style.Add("Display", "none");
                celula.Controls.Add(selecionaData);
    
                CustomValidator validaData = new CustomValidator();
                validaData.ID = parcela.ID + "_ctvValidaData";
                validaData.ControlToValidate = campoData.ID;
                validaData.ValidateEmptyText = true;
                validaData.ClientValidationFunction = "validaDataP";
                validaData.Display = ValidatorDisplay.Dynamic;
                validaData.Text = " *";
                validaData.ErrorMessage = (i + 1) + "ª parcela: Data não pode ser vazia ou inferior à data da parcela anterior.";
                validaData.EnableViewState = this.EnableViewState;
                Page.Validators.Add(validaData);
                celula.Controls.Add(validaData);
    
                linhasParcela[2].Cells.Add(celula);
    
                #endregion
    
                #region Cria as células do controle adicional
    
                if (_exibirCampoAdicional)
                {
                    celula = new TableCell();
                    celula.Text = _tituloCampoAdicional;
                    linhasParcela[4].Cells.Add(celula);
    
                    celula = new TableCell();
                    TextBox campoAdicional = new TextBox();
                    campoAdicional.ID = parcela.ID + "_txtAdicional";
                    campoAdicional.Width = new Unit("80px");
                    campoAdicional.EnableViewState = this.EnableViewState;
                    campoAdicional.Enabled = !_readOnly;
                    celula.Controls.Add(campoAdicional);
                    _camposAdicionais[i] = campoAdicional;
                    linhasParcela[4].Cells.Add(celula);
                }
    
                #endregion
    
                #region Cria a célula de juros
    
                if (_calcularJurosParcela)
                {
                    celula = new TableCell();
                    celula.ColumnSpan = 2;
                    CheckBox calcularJuros = new CheckBox();
                    calcularJuros.ID = parcela.ID + "_chkJuros";
                    calcularJuros.Text = "Aplicar taxa de juros";
                    calcularJuros.Attributes.Add("OnClick", "Parc_calculaJuros('" + this.ClientID + "', " + CallbackTotal + ", " + CallbackAplicarJuros + ")");
                    calcularJuros.Checked = true;
                    celula.Controls.Add(calcularJuros);
    
                    linhasParcela[numLinhas - (_isCompra ? 2 : 1)].Cells.Add(celula);
                }
    
                #endregion
    
                #region Cria as células de forma de pagamento
    
                if (_isCompra)
                {
                    celula = new TableCell();
                    celula.Text = "Forma Pagto.";
                    linhasParcela[numLinhas - 1].Cells.Add(celula);
    
                    celula = new TableCell();
                    DropDownList formaPagto = new DropDownList();
                    formaPagto.ID = parcela.ID + "_drpFormaPagto";
                    formaPagto.EnableViewState = this.EnableViewState;
                    formaPagto.Enabled = !_readOnly;
                    formaPagto.DataSourceID = odsFormaPagto.ID;
                    formaPagto.DataTextField = "Descricao";
                    formaPagto.DataValueField = "IdFormaPagto";
                    celula.Controls.Add(formaPagto);
                    _formasPagto[i] = formaPagto;
                    linhasParcela[numLinhas - 1].Cells.Add(celula);
                }
    
                #endregion
    
                // Adiciona a linha à tabela
                parcela.Controls.Add(dadosParcela);
                linha.Cells.Add(parcela);
            }
    
            // Salva a linha na tabela
            tblParcelas.Rows.Add(linha);
        }
    
        protected void ctrlParcelas_PreRender(object sender, EventArgs e)
        {
            // String com o formato da variável
            string formato = "" +
                "TotalParcelas: {0}, " +
                "ParentID: '{1}', " +
                "CampoValorTotal: '{2}', " +
                "CampoValorEntrada: '{3}', " +
                "CampoParcelasVisiveis: '{4}', " +
                "CampoExibirParcelas: '{5}', " +
                "CampoCalcularParcelas: '{6}', " +
                "CampoValorDescontoAtual: '{7}', " +
                "CampoValorDescontoAnterior: '{8}', " +
                "CampoValorAcrescimoAtual: '{9}', " +
                "CampoValorAcrescimoAnterior: '{10}', " +
                "CampoTextoParcelas: '{11}', " +
                "CampoDataBase: '{12}', " +
                "CampoTaxaJurosParcela: '{13}', " +
                "CampoTipoDescontoAtual: '{14}', " +
                "CampoTipoDescontoAnterior: '{15}', " +
                "CampoTipoAcrescimoAtual: '{16}', " +
                "CampoTipoAcrescimoAnterior: '{17}', " +
                "Habilitado: {18}, " +
                "Valores: {19}, " +
                "Datas: {20}, " +
                "Adicionais: {21}, " +
                "Juros: {22}, " +
                "JurosCompostos: {23}, " +
                "AplicarJuros: {24}, " +
                "DiasParcelas: {25}, " +
                "NumeroParcelas: {26}, " +
                "Habilitar: {27}, " +
                "Calcular: {28}, " +
                "CampoValorObra: '{29}', " +
                "CampoFormaPagto: '{30}', " +
                "IsCompra: {31}, " +
                "FormasPagamento: {32}, " +
                "AlterarData: {33}, " +
                "CalcularData: {34}, " + 
                "LiberarPedido: {35}, " +
                "ExibirValores: {36}, " +
                "DiasSomarDataVazia: {37}";
    
            // Formata os controles de campo
            FormatControl(_campoValorTotal);
            FormatControl(_campoValorEntrada);
            FormatControl(_campoParcelasVisiveis);
            FormatControl(_campoExibirParcelas);
            FormatControl(_campoCalcularParcelas);
            FormatControl(_campoValorDescontoAtual);
            FormatControl(_campoValorDescontoAnterior);
            FormatControl(_campoValorAcrescimoAtual);
            FormatControl(_campoValorAcrescimoAnterior);
            FormatControl(_campoTextoParcelas);
            FormatControl(_campoDataBase);
            FormatControl(_campoTaxaJurosParcela);
            FormatControl(_campoTipoDescontoAtual);
            FormatControl(_campoTipoDescontoAnterior);
            FormatControl(_campoTipoAcrescimoAtual);
            FormatControl(_campoTipoAcrescimoAnterior);
            FormatControl(_campoValorObra);
            FormatControl(_campoFormaPagto, "Parc_atualizaFormasPagto('" + this.ClientID + "')");
    
            // Variável que contém os dados que serão usados para criação da variável do controle
            object[] dadosFormato = new object[38];
            dadosFormato[0] = NumParcelas;
            dadosFormato[1] = _parentId;
            dadosFormato[2] = GetControlID(_campoValorTotal);
            dadosFormato[3] = GetControlID(_campoValorEntrada);
            dadosFormato[4] = GetControlID(_campoParcelasVisiveis);
            dadosFormato[5] = GetControlID(_campoExibirParcelas);
            dadosFormato[6] = GetControlID(_campoCalcularParcelas);
            dadosFormato[7] = GetControlID(_campoValorDescontoAtual);
            dadosFormato[8] = GetControlID(_campoValorDescontoAnterior);
            dadosFormato[9] = GetControlID(_campoValorAcrescimoAtual);
            dadosFormato[10] = GetControlID(_campoValorAcrescimoAnterior);
            dadosFormato[11] = GetControlID(_campoTextoParcelas);
            dadosFormato[12] = GetControlID(_campoDataBase);
            dadosFormato[13] = GetControlID(_campoTaxaJurosParcela);
            dadosFormato[14] = GetControlID(_campoTipoDescontoAtual);
            dadosFormato[15] = GetControlID(_campoTipoDescontoAnterior);
            dadosFormato[16] = GetControlID(_campoTipoAcrescimoAtual);
            dadosFormato[17] = GetControlID(_campoTipoAcrescimoAnterior);
            dadosFormato[18] = (!_readOnly).ToString().ToLower();
            dadosFormato[19] = "function(asString) { return getValoresParcelas('" + this.ClientID + "', asString); }";
            dadosFormato[20] = "function(asString) { return getDatasParcelas('" + this.ClientID + "', asString); }";
            dadosFormato[21] = "function(asString) { return getAdicionaisParcelas('" + this.ClientID + "', asString); }";
            dadosFormato[22] = "function(asString) { return getJurosParcelas('" + this.ClientID + "', asString); }";
            dadosFormato[23] = FinanceiroConfig.FormaPagamento.AcumularJurosParcelasTaxaPrazo.ToString().ToLower();
            dadosFormato[24] = _calcularJurosParcela.ToString().ToLower();
            dadosFormato[25] = "function(asString) { return getNumeroDiasParcelas('" + this.ClientID + "', asString); }";
            dadosFormato[26] = "function() { return Parc_getParcelasVisiveis('" + this.ClientID + "'); }";
            dadosFormato[27] = "function(habilitar) { return Parc_habilitar('" + this.ClientID + "', habilitar, false); }";
            dadosFormato[28] = "function() { " + GetFuncaoCalculo() + " }";
            dadosFormato[29] = GetControlID(_campoValorObra);
            dadosFormato[30] = GetControlID(_campoFormaPagto);
            dadosFormato[31] = _isCompra.ToString().ToLower();
            dadosFormato[32] = "function(asString) { return getFormasPagamento('" + this.ClientID + "', asString) }";
            dadosFormato[33] = (!_readOnly && _alterarData).ToString().ToLower();
            dadosFormato[34] = _calcularData.ToString().ToLower();
            dadosFormato[35] = _liberarPedido.ToString().ToLower();
            dadosFormato[36] = _exibirValores.ToString().ToLower();
            dadosFormato[37] = _diasSomarDataVazia;
    
            // Registra a variável na tela
            string script = "var " + this.ClientID + " = { " + String.Format(formato, dadosFormato) + " };\n";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, script, true);
    
            // Registra o script de inicialização do controle na tela
            script = GetFuncaoCalculo() + ";\n";
            if (_campoCalcularParcelas != null)
                script += "document.getElementById('" + GetControlID(_campoCalcularParcelas) + "').value = true;\n";
    
            script += "Parc_atualizarValorTotal('" + this.ClientID + "', " + CallbackTotal + ");\n";
    
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_Load", script, true);
    
            // Define o script que será executado ao fazer submit
            Page.ClientScript.RegisterOnSubmitStatement(GetType(), this.ClientID + "_Submit", "Parc_habilitar('" + this.ClientID + "', true, true)");
        }
    }
}
