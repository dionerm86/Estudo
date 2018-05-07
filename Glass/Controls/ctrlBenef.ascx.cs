using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Glass.Data.Model;
using System.Collections.Generic;
using Glass.Data.DAL;
using System.Text;
using Glass.Data.Helper;
using System.Reflection;
using Glass.Configuracoes;
using System.Linq;
using Colosoft;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlBenef : BaseUserControl
    {
        #region Campos privados
    
        private string _callbackSelecaoItem;
        private string _callbackCalculoValorItem;
        private string _callbackCalculoValorTotal;
        private string _funcaoValorCalculoAdicional;
        private string _cssClassCabecalho;
        private string _cssClassLinha;
        private string _cssClassLinhaAlternada;
        private bool _calcularValorAdicionalBenef = false;
        private bool _bloquearBeneficiamentos = false;
        private string _propriedadeIdAntigo;
        private bool _exibirValorBeneficiamento = false;
        private string _mensagemBloqueioBenef;
        private bool _isCompra = false;
        private bool _carregarBenefPadrao = true;
        private bool _somarTotalValorBenef = false;
        private bool _calcularBeneficiamentoPadrao = false;
    
        private Control _campoEspessura;
        private Control _campoTipoEntrega;
        private Control _campoPercComissao;
        private Control _campoAltura;
        private Control _campoLargura;
        private Control _campoTotalM2;
        private Control _campoValorUnitario;
        private Control _campoCusto;
        private Control _campoQuantidade;
        private Control _campoQuantidadeAmbiente;
        private Control _campoIdProduto;
        private Control _campoRevenda;
        private Control _campoIdCliente;
        private Control _campoIdPecaItemProjeto;
        private Control _campoIdAplicacao;
        private Control _campoIdProcesso;
        private Control _campoAplicacao;
        private Control _campoProcesso;

        private TipoBenef _tipoBenef = TipoBenef.Todos;
        private List<KeyValuePair<int, KeyValuePair<int, int>>> itensNenhum;

        private IEnumerable<Glass.Global.Negocios.Entidades.IBeneficiamento> _beneficiamentos2;
        private string _tipoBeneficiamento;
    
        #endregion
    
        #region Métodos dos controles
    
        /// <summary>
        /// Retorna os controles do bisotê.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsBisote(BenefConfig benef, string prefixo)
        {
            // Cria o Label de espessura
            Label lblEspessura = new Label();
            lblEspessura.ID = prefixo + "lblEspessura";
            lblEspessura.Text = "Esp.";
    
            // Cria o TextBox de espessura
            TextBox txtEspessura = new TextBox();
            txtEspessura.ID = prefixo + "txtEspessura";
            txtEspessura.Width = new Unit("30px");
            txtEspessura.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true)");
            txtEspessura.Enabled = !_bloquearBeneficiamentos;
            txtEspessura.EnableViewState = this.EnableViewState;
    
            // Cria o CustomValidator para a espessura do bisotê
            CustomValidator ctvEspessura = new CustomValidator();
            ctvEspessura.ID = prefixo + "ctvEspessura";
            ctvEspessura.ClientValidationFunction = "validaEspessuraBisote";
            ctvEspessura.ControlToValidate = txtEspessura.ID;
            ctvEspessura.Display = ValidatorDisplay.None;
            ctvEspessura.ErrorMessage = "A espessura do \"" + benef.DescricaoCompleta + "\" deve ser informada.";
            ctvEspessura.ValidateEmptyText = true;
            ctvEspessura.ValidationGroup = vsuSumario.ValidationGroup;
    
            // Recupera os controles da lapidação
            WebControl[] lapidacao = GetControlsLapidacao(benef, prefixo);
    
            // Cria o vetor de retorno
            WebControl[] retorno = new WebControl[lapidacao.Length + 3];
    
            // Adiciona os controles da lapidação ao vetor
            for (int i = 0; i < lapidacao.Length; i++)
                retorno[i] = lapidacao[i];
    
            // Adiciona os controles de espessura ao retorno
            retorno[lapidacao.Length] = lblEspessura;
            retorno[lapidacao.Length + 1] = txtEspessura;
            retorno[lapidacao.Length + 2] = ctvEspessura;
    
            // Retorna os controles
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controles da lapidação.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsLapidacao(BenefConfig benef, string prefixo)
        {
            // Cria o DropDownList de altura
            DropDownList drpAltura = new DropDownList();
            drpAltura.ID = prefixo + "drpAltura";
            drpAltura.Items.Add(new ListItem("0", "0"));
            drpAltura.Items.Add(new ListItem("1", "1"));
            drpAltura.Items.Add(new ListItem("2", "2"));
            drpAltura.Enabled = !_bloquearBeneficiamentos;
            drpAltura.EnableViewState = this.EnableViewState;
    
            // Cria o DropDownList de largura
            DropDownList drpLargura = new DropDownList();
            drpLargura.ID = prefixo + "drpLargura";
            drpLargura.Items.Add(new ListItem("0", "0"));
            drpLargura.Items.Add(new ListItem("1", "1"));
            drpLargura.Items.Add(new ListItem("2", "2"));
            drpLargura.Enabled = !_bloquearBeneficiamentos;
            drpLargura.EnableViewState = this.EnableViewState;
    
            // Recupera os controles da lista de seleção
            WebControl[] listaSelecao = GetControlsListaSelecao(benef, prefixo, true);
    
            // Cria o vetor de retorno
            WebControl[] retorno = new WebControl[listaSelecao.Length + 2];
    
            // Adiciona os controles da lista de seleção ao vetor
            for (int i = 0; i < listaSelecao.Length; i++)
                retorno[i] = listaSelecao[i];
    
            // Adiciona os controles de altura e largura ao vetor
            retorno[listaSelecao.Length] = PedidoConfig.EmpresaTrabalhaAlturaLargura ? drpAltura : drpLargura;
            retorno[listaSelecao.Length + 1] = PedidoConfig.EmpresaTrabalhaAlturaLargura ? drpLargura : drpAltura;
    
            // Retorna os controles
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controles da lista de seleção.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsListaSelecao(BenefConfig benef, string prefixo, bool addVazio)
        {
            // Cria o DropDownList de tipo
            DropDownList drpTipo = new DropDownList();
            drpTipo.ID = prefixo + "drpTipo";
            drpTipo.Enabled = !_bloquearBeneficiamentos;
            drpTipo.EnableViewState = this.EnableViewState;
            SetDropDownListData(benef, drpTipo, addVazio);
    
            // Cria e retorna o vetor de controles
            WebControl[] retorno = new WebControl[1];
            retorno[0] = drpTipo;
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controles da lista de seleção e quantidade.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsListaSelecaoQtd(BenefConfig benef, string prefixo)
        {
            // Recupera os controles de lista de seleção e de quantidade
            WebControl[] listaSelecao = GetControlsListaSelecao(benef, prefixo, false);
            WebControl[] quantidade = GetControlsQuantidade(benef, prefixo);
            
            // Cria uma tabela para organizar os controles
            Table tblListaSelecaoQtd = new Table();
            tblListaSelecaoQtd.CellPadding = 0;
            tblListaSelecaoQtd.CellSpacing = 0;
            tblListaSelecaoQtd.ID = prefixo + "tblListaSelecaoQtd";
            tblListaSelecaoQtd.EnableViewState = this.EnableViewState;
            tblListaSelecaoQtd.Style.Add("padding", "0px");
            tblListaSelecaoQtd.Style.Add("border-collapse", "collapse");
            tblListaSelecaoQtd.Style.Add("display", "inline");
            
            TableRow linha = new TableRow();
            tblListaSelecaoQtd.Rows.Add(linha);
            TableCell itens = new TableCell();
            for (var i = 0; i < listaSelecao.Length; i++)
                itens.Controls.Add(listaSelecao[i]);
            TableCell qtd = new TableCell();
            for (var i = 0; i < quantidade.Length; i++)
                qtd.Controls.Add(quantidade[i]);
            linha.Cells.AddRange(new TableCell[] { itens, qtd });
            // Cria e retorna o vetor com os controles
            WebControl[] retorno = new WebControl[1];
            retorno[0] = tblListaSelecaoQtd;
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controles da quantidade.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsQuantidade(BenefConfig benef, string prefixo)
        {
            // Cria uma tabela, com 2 linhas e 2 colunas
            Table tblQtd = new Table();
            TableRow cima = new TableRow();
            TableRow baixo = new TableRow();
            tblQtd.EnableViewState = this.EnableViewState;
            tblQtd.Rows.AddRange(new TableRow[] {cima, baixo});
            TableCell qtd = new TableCell();
            qtd.RowSpan = 2;
            qtd.Style.Add("padding", "0px");
            TableCell botaoCima = new TableCell();
            botaoCima.Style.Add("padding", "0px");
            botaoCima.Style.Add("vertical-align", "bottom");
            cima.Cells.AddRange(new TableCell[] {qtd, botaoCima});
            TableCell botaoBaixo = new TableCell();
            botaoBaixo.Style.Add("padding", "0px");
            botaoBaixo.Style.Add("vertical-align", "top");
            baixo.Cells.Add(botaoBaixo);
            tblQtd.Style.Add("padding", "0px");
            tblQtd.Style.Add("border-collapse", "collapse");
            tblQtd.Style.Add("display", "inline");
            tblQtd.ID = prefixo + "tblQtd";

            // Cria o TextBox de quantidade
            TextBox txtQtd = new TextBox();
            txtQtd.ID = tblQtd.ID + "_txtQtd";
            txtQtd.Text = "0";
            txtQtd.Width = new Unit("30px");
            txtQtd.Attributes.Add("OnKeyPress", "return soNumeros(event, true, true)");
            txtQtd.Enabled = !_bloquearBeneficiamentos;
            txtQtd.EnableViewState = this.EnableViewState;
            txtQtd.Style.Add("margin", "0px");
            qtd.Controls.Add(txtQtd);

            // Cria o ImageButton com a seta para cima
            ImageButton imbUp = new ImageButton();
            imbUp.ID = tblQtd.ID + "_imbUp";
            imbUp.ImageUrl = "~/Images/numUp.gif";
            imbUp.OnClientClick = "numUpDown('" + this.ClientID + "_" + txtQtd.ID + "', 'up'); return false";
            imbUp.Width = new Unit("15px");
            imbUp.Enabled = !_bloquearBeneficiamentos;
            imbUp.EnableViewState = false;
            imbUp.Style.Value = "margin: 0px";
            botaoCima.Controls.Add(imbUp);
                
            // Cria o ImageButton com a seta para baixo
            ImageButton imbDown = new ImageButton();
            imbDown.ID = tblQtd.ID + "_imbDown";
            imbDown.ImageUrl = "~/Images/numDown.gif";
            imbDown.OnClientClick = "numUpDown('" + this.ClientID + "_" + txtQtd.ID + "', 'down'); return false";
            imbDown.Width = new Unit("15px");
            imbDown.Enabled = !_bloquearBeneficiamentos;
            imbDown.EnableViewState = false;
            imbDown.Style.Value = "margin: 0px";
            botaoBaixo.Controls.Add(imbDown);

            // Cria e retorna o vetor com os controles
            WebControl[] retorno = new WebControl[1];
            retorno[0] = tblQtd;
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controles da seleção múltipla exclusiva.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsSelecaoMultiplaExclusiva(BenefConfig benef, string prefixo)
        {
            // Recupera os controles da lista de seleção inclusiva
            WebControl[] selecaoMultiplaExclusiva = GetControlsSelecaoMultiplaInclusiva(benef, prefixo);
    
            // Adiciona um JavaScript para fazer com que apenas um controle seja marcado de cada vez
            foreach (WebControl c in selecaoMultiplaExclusiva)
                c.Attributes.Add("OnClick", "selecaoUnica('" + this.ClientID + "_" + prefixo + "', this)");
    
            // Retorna o vetor de controles
            return selecaoMultiplaExclusiva;
        }
    
        /// <summary>
        /// Retorna os controles da seleção múltipla inclusiva.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsSelecaoMultiplaInclusiva(BenefConfig benef, string prefixo)
        {
            // Recupera a lista de itens
            var itens = GetSubItens(benef);
    
            // Cria o vetor de retorno
            var retorno = new WebControl[itens.Count];
    
            // Cria um CheckBox para cada item da lista e o adiciona ao vetor
            for (int i = 0; i < itens.Count; i++)
            {
                CheckBox chkSelecao = new CheckBox();
                chkSelecao.ID = prefixo + "chkSelecao" + (i + 1).ToString();
                chkSelecao.Text = itens[i].Nome;
                chkSelecao.Attributes.Add("idBeneficiamento", itens[i].IdBenefConfig.ToString());
                chkSelecao.Enabled = !_bloquearBeneficiamentos;
                chkSelecao.EnableViewState = this.EnableViewState;
    
                retorno[i] = chkSelecao;
            }
    
            // Retorna o vetor
            return retorno;
        }
    
        /// <summary>
        /// Retorna os controels da seleção simples.
        /// </summary>
        /// <returns></returns>
        private WebControl[] GetControlsSelecaoSimples(BenefConfig benef, string prefixo)
        {
            // Cria o Checkbox do item
            CheckBox chkSelecao = new CheckBox();
            chkSelecao.ID = prefixo + "chkSelecao";
            chkSelecao.Enabled = !_bloquearBeneficiamentos;
            chkSelecao.EnableViewState = this.EnableViewState;
    
            // Cria e retorna o vetor de controles
            WebControl[] retorno = new WebControl[1];
            retorno[0] = chkSelecao;
            return retorno;
        }
    
        #endregion
    
        #region Métodos de suporte
    
        /// <summary>
        /// Indica se o browser que exibe o controle é o Chrome.
        /// </summary>
        /// <returns></returns>
        private bool IsChrome()
        {
            return this.Page.Request.UserAgent.Contains("Chrome");
        }
    
        /// <summary>
        /// Retorna o controle 'Redondo'.
        /// </summary>
        /// <returns></returns>
        private CheckBox GetControlRedondo()
        {
            // Retorna o controle 'Redondo'
            return (CheckBox)tblBenef.FindControl("Redondo_chkSelecao");
        }
    
        /// <summary>
        /// Retorna os itens vazios formatados como vetor de objetos.
        /// </summary>
        /// <returns></returns>
        private string GetItensVazios()
        {
            // String com o formato do objeto de retorno
            string formato = "" +
                "ID: {0}, " +
                "TipoControle: {1}, " +
                "TipoCalculo: {2}, " +
                "TipoEspessura: 0";
    
            // Variável de retorno
            StringBuilder retorno = new StringBuilder();
    
            // Cria um objeto no retorno para cada item vazio
            foreach (KeyValuePair<int, KeyValuePair<int, int>> item in itensNenhum)
            {
                // Variável com os dados usados para formatar a string
                object[] dadosFormato = new object[3];
                dadosFormato[0] = item.Key;
                dadosFormato[1] = item.Value.Key;
                dadosFormato[2] = item.Value.Value;
    
                retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
            }
    
            return "new Array(" + (retorno.Length > 0 ? retorno.ToString().Substring(2) : "") + ")";
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
                // Recupera a função do controle
                funcao = ((WebControl)campo).Attributes[atributo];
    
                // Verifica se a função desejada já está no controle
                if (funcao.IndexOf(GetFuncaoCalculo(null)) > -1)
                    return;
    
                // Coloca a função de cálculo junto à função original
                if (funcao.IndexOf("return") > -1)
                    funcao = funcao.Replace("return", GetFuncaoCalculo(null) + "; return");
                else
                    funcao += "; " + GetFuncaoCalculo(null);
            }
    
            // Indica que apenas essa função será executada
            else
                funcao = GetFuncaoCalculo(null);
    
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
        /// Retorna o prefixo dos nomes dos controles.
        /// </summary>
        /// <param name="benef"></param>
        /// <returns></returns>
        private string PrefixoControles(BenefConfig benef)
        {
            // Retorna uma string com o nome do beneficiamento formatado
            return benef.Nome.Trim().Replace(" ", "_").Replace("²", "2") + "_";
        }
    
        /// <summary>
        /// Cria os beneficiamentos nas células.
        /// </summary>
        /// <param name="cabecalho">A célula do cabeçalho.</param>
        /// <param name="controles">A célula dos controles.</param>
        /// <param name="benef">O beneficiamento.</param>
        private void CreateBenef(TableCell cabecalho, TableCell controles, BenefConfig benef)
        {
            // Cria um Label com o nome do beneficiamento para o cabeçalho
            Label lblNome = new Label();
            lblNome.ID = PrefixoControles(benef) + "lblNome";
            lblNome.Text = benef.Nome;
            cabecalho.Controls.Add(lblNome);
    
            // Verifica se a cobrança é opcional
            if (benef.CobrancaOpcional)
            {
                // Cria um CheckBox para a cobrança opcional
                CheckBox chkOpcional = new CheckBox();
                chkOpcional.ID = PrefixoControles(benef) + "chkOpcional";
                chkOpcional.Text = "Cobrar?";
                chkOpcional.Checked = !OrcamentoConfig.CheckBenefOpcionalDesmascadoPadrao;
                chkOpcional.Attributes.Add("IdBeneficiamento", benef.IdBenefConfig.ToString());
                chkOpcional.Attributes.Add("OnClick", GetFuncaoCalculo(benef));
                chkOpcional.Style.Add("Float", "right");
                chkOpcional.Style.Add("Position", "relative");
                chkOpcional.Style.Add("Bottom", !IsChrome() ? "14px" : "2px");
                chkOpcional.Style.Add("Margin-bottom", !IsChrome() ? "-19px" : "-7px");
                chkOpcional.Style.Add("Font-weight", "normal");
                chkOpcional.Style.Add("Font-size", "85%");
                chkOpcional.Style.Add("Padding-left", "7px");
                chkOpcional.Style.Add("Padding-right", "2px");
                cabecalho.Controls.Add(chkOpcional);
            }
    
            // Indica na célula do cabeçalho se a cobrança é opcional
            cabecalho.Attributes.Add("Opcional", benef.CobrancaOpcional.ToString().ToLower());
    
            // Formata a célula do cabeçalho
            cabecalho.Style.Add("White-space", "nowrap");
            cabecalho.Style.Add("Padding", "3px");
            if (!String.IsNullOrEmpty(_cssClassCabecalho))
                cabecalho.CssClass = _cssClassCabecalho;
    
            // Indica na célula dos controles o ID do beneficiamento
            controles.Attributes.Add("IdBeneficiamento", benef.IdBenefConfig.ToString());
    
            // Formata a célula dos controles
            controles.Style.Add("White-space", "nowrap");
            controles.Style.Add("Padding-right", "4px");
    
            // Adiciona os controles à célula
            foreach (Control c in GetControls(benef))
                controles.Controls.Add(c);
    
            // Cria o HiddenField que indica o ID do beneficiamento aplicado
            HiddenField hdfIdBenefAplicado = new HiddenField();
            hdfIdBenefAplicado.ID = PrefixoControles(benef) + "hdfIdBenefAplicado";
            hdfIdBenefAplicado.Value = "0";
    
            // Cria o HiddenField com o valor unitário do beneficiamento
            HiddenField hdfValorUnit = new HiddenField();
            hdfValorUnit.ID = PrefixoControles(benef) + "hdfValorUnit1";
            hdfValorUnit.Value = "0";
    
            // Cria o HiddenField com o valor do beneficiamento
            HiddenField hdfValor = new HiddenField();
            hdfValor.ID = PrefixoControles(benef) + "hdfValor1";
            hdfValor.Value = "0";
    
            // Cria o HiddenField com o custo do beneficiamento
            HiddenField hdfCusto = new HiddenField();
            hdfCusto.ID = PrefixoControles(benef) + "hdfCusto1";
            hdfCusto.Value = "0";
    
            // Cria o HiddenField com a descrição do beneficiamento
            HiddenField hdfDescricao = new HiddenField();
            hdfDescricao.ID = PrefixoControles(benef) + "hdfDescricao";
    
            // Cria o HiddenField com os dados do serviço
            HiddenField hdfInfo = new HiddenField();
            hdfInfo.ID = PrefixoControles(benef) + "hdfInfo";
    
            // Cria o HiddenField que indica se o beneficiamento é padrão do produto
            HiddenField hdfBenefPadrao = new HiddenField();
            hdfBenefPadrao.ID = PrefixoControles(benef) + "hdfBenefPadrao";
            hdfBenefPadrao.Value = "false";
    
            // Cria o HiddenField que indica se o beneficiamento é associado
            HiddenField hdfNaoCobrarBenef = new HiddenField();
            hdfNaoCobrarBenef.ID = PrefixoControles(benef) + "hdfNaoCobrarBenef";
            hdfNaoCobrarBenef.Value = "false";
    
            // Adiciona os HiddenFields à célula de controles
            controles.Controls.Add(hdfIdBenefAplicado);
            controles.Controls.Add(hdfValorUnit);
            controles.Controls.Add(hdfValor);
            controles.Controls.Add(hdfCusto);
            controles.Controls.Add(hdfDescricao);
            controles.Controls.Add(hdfInfo);
            controles.Controls.Add(hdfBenefPadrao);
            controles.Controls.Add(hdfNaoCobrarBenef);
    
            // Se o tipo de controle do beneficiamento for SelecaoMultiplaInclusiva
            if (benef.TipoControle == TipoControleBenef.SelecaoMultiplaInclusiva)
            {
                // Lista com os campos que serão criados
                List<HiddenField> valoresCustos = new List<HiddenField>();
    
                // Cria um controle de valor e custo para cada controle de seleção
                for (int i = 1; i < benef.NumChild; i++)
                {
                    // Cria o HiddenField de valor unitário
                    HiddenField hdfNovoValorUnit = new HiddenField();
                    hdfNovoValorUnit.ID = PrefixoControles(benef) + "hdfValorUnit" + (i + 1);
                    hdfNovoValorUnit.Value = "0";
    
                    // Cria o HiddenField de valor
                    HiddenField hdfNovoValor = new HiddenField();
                    hdfNovoValor.ID = PrefixoControles(benef) + "hdfValor" + (i + 1);
                    hdfNovoValor.Value = "0";
    
                    // Cria o HiddenField de custo
                    HiddenField hdfNovoCusto = new HiddenField();
                    hdfNovoCusto.ID = PrefixoControles(benef) + "hdfCusto" + (i + 1);
                    hdfNovoCusto.Value = "0";
    
                    // Adiciona os controles à lista
                    valoresCustos.AddRange(new HiddenField[] { hdfNovoValorUnit, hdfNovoValor, hdfNovoCusto });
                }
    
                // Adiciona os controles da lista à célula de controles
                foreach (HiddenField h in valoresCustos)
                    controles.Controls.Add(h);
            }
        }
    
        /// <summary>
        /// Retorna a lista de controles do beneficiamento.
        /// </summary>
        /// <param name="benef">O beneficiamento que será criado.</param>
        /// <returns>Um vetor com os controles do beneficiamento.</returns>
        private Control[] GetControls(BenefConfig benef)
        {
            // Variável de retorno
            List<Control> retorno = new List<Control>();
            string prefixo = PrefixoControles(benef);
    
            // Retorna os controles de acordo com o tipo definido no beneficiamento
            switch (benef.TipoControle)
            {
                case TipoControleBenef.Bisote:
                    retorno.AddRange(GetControlsBisote(benef, prefixo));
                    break;
                
                case TipoControleBenef.Lapidacao:
                    retorno.AddRange(GetControlsLapidacao(benef, prefixo));
                    break;
                
                case TipoControleBenef.ListaSelecao:
                    retorno.AddRange(GetControlsListaSelecao(benef, prefixo, true));
                    break;
                
                case TipoControleBenef.ListaSelecaoQtd:
                    retorno.AddRange(GetControlsListaSelecaoQtd(benef, prefixo));
                    break;
                
                case TipoControleBenef.Quantidade:
                    retorno.AddRange(GetControlsQuantidade(benef, prefixo));
                    break;
                
                case TipoControleBenef.SelecaoMultiplaExclusiva:
                    retorno.AddRange(GetControlsSelecaoMultiplaExclusiva(benef, prefixo));
                    break;
                
                case TipoControleBenef.SelecaoMultiplaInclusiva:
                    retorno.AddRange(GetControlsSelecaoMultiplaInclusiva(benef, prefixo));
                    break;
                
                case TipoControleBenef.SelecaoSimples:
                    retorno.AddRange(GetControlsSelecaoSimples(benef, prefixo));
                    break;
                
                default:
                    retorno.AddRange(GetControlsSelecaoSimples(benef, prefixo));
                    break;
            }
    
            if (_exibirValorBeneficiamento)
            {
                // Cria o SPAN que contém o valor
                var span = new HtmlGenericControl("span");
                span.Style.Add("float", "right");
                if (!IsChrome()) span.Style.Add("Position", "relative");
                span.Style.Add("Bottom", "19px");
                span.Style.Add("Margin-bottom", "-24px");
                span.Style.Add("Padding-left", "5px");
                span.Style.Add("Text-align", "right");
    
                // Cria os label com a descrição do beneficiamento
                var lblValorBenef = new Label();
                lblValorBenef.ID = prefixo + "lblTituloValorBenef";
                lblValorBenef.Text = "&nbsp;Valor " + benef.DescrTipoCalculo + (benef.TipoCalculo != TipoCalculoBenef.Porcentagem ? " R$&nbsp;" : " ");
    
                // Cria o controle com o valor do beneficiamento
                var precos = BenefConfigPrecoDAO.Instance.GetByIdBenefConfig((uint)benef.IdBenefConfig);
                var txtValorBenef = new TextBox();
                txtValorBenef.ID = prefixo + "txtValorBenef";
                txtValorBenef.CssClass = "valorBenef";
                txtValorBenef.Attributes.Add("OnKeyPress", "return soNumeros(event, false, true)");
                txtValorBenef.Attributes.Add("OnChange", GetFuncaoCalculo(benef));
                txtValorBenef.Text = precos.Count > 0 ? precos[0].Custo.ToString("0.00") : "0,00";
    
                // Adiciona os controles ao SPAN
                span.Controls.Add(lblValorBenef);
                span.Controls.Add(txtValorBenef);
    
                // Adiciona o SPAN à lista
                retorno.Add(span);
            }
    
            if (!String.IsNullOrEmpty(_propriedadeIdAntigo))
            {
                try
                {
                    // Cria o controle com o ID do beneficiamento antigo
                    HiddenField hdfIdAntigo = new HiddenField();
                    hdfIdAntigo.ID = prefixo + "hdfIdAntigo";
                    retorno.Add(hdfIdAntigo);
                }
                catch { }
            }
    
            // Altera algumas propriedades dos controles
            SetControlsParameters(benef, retorno);
    
            // Retorna o vetor
            return retorno.ToArray();
        }
    
        /// <summary>
        /// Retorna os sub-itens de um beneficiamento.
        /// </summary>
        /// <param name="benef">O beneficiamento pai.</param>
        /// <returns>Uma lista com os beneficiamentos vinculados ao pai.</returns>
        private IList<BenefConfig> GetSubItens(BenefConfig benef)
        {
            // Recupera os itens filhos de um beneficiamento
            return BenefConfigDAO.Instance.GetByBenefConfig((uint)benef.IdBenefConfig);
        }
    
        /// <summary>
        /// Cria uma ligação com o banco de dados no DropDownList.
        /// </summary>
        /// <param name="benef">O beneficiamento pai.</param>
        /// <param name="controle">O controle que será ligado.</param>
        /// <param name="addVazio">Deve ser adicionado um item vazio no começo da lista?</param>
        private void SetDropDownListData(BenefConfig benef, DropDownList controle, bool addVazio)
        {
            // Limpa os itens do controle
            controle.Items.Clear();
    
            // Verifica se será adicionado um item vazio à lista
            if (addVazio)
            {
                // Adiciona um item à lista de itens vazios
                itensNenhum.Add(new KeyValuePair<int,KeyValuePair<int,int>>(-(itensNenhum.Count + 1), new KeyValuePair<int,int>((int)benef.TipoControle, (int)benef.TipoCalculo)));
                
                // Cria o item vazio
                ListItem itemVazio = new ListItem();
                itemVazio.Value = (itensNenhum[itensNenhum.Count - 1].Key).ToString();
                itemVazio.Text = "";
    
                // Adiciona o item ao controle
                controle.Items.Add(itemVazio);
            }
    
            // Adiciona um item ao controle para cada item filho do beneficiamento
            foreach (BenefConfig b in GetSubItens(benef))
                controle.Items.Add(new ListItem(b.Nome, b.IdBenefConfig.ToString()));
        }
    
        /// <summary>
        /// Muda algumas configurações do controle para que ele fique padronizado.
        /// </summary>
        /// <param name="benef">O beneficiamento que será criado.</param>
        /// <param name="controles">O controle que será alterado.</param>
        private void SetControlsParameters(BenefConfig benef, IEnumerable<Control> controles)
        {
            // Altera todos os controles
            foreach (Control c in controles)
            {
                if (!(c is WebControl))
                    continue;
    
                // Se o controle não for um ImageButton ou um Table adiciona um espaço à sua esquerda
                if (!(c is ImageButton) && !(c is Table))
                    ((WebControl)c).Style.Add("margin-left", "3px");
    
                // Se o controle não for um CheckBox ou um ImageButton
                if (!(c is CheckBox) && !(c is ImageButton))
                {
                    // Se não for um Table
                    if (!(c is Table))
                        ((WebControl)c).Attributes.Add("OnChange", GetFuncaoCalculo(benef));
    
                    // Altera os controles que estão dentro da tabela
                    else
                    {
                        foreach (TableRow linha in ((Table)c).Rows)
                            foreach (TableCell celula in linha.Cells)
                            {
                                // Procura todos os controles que deverão ser mudados
                                List<WebControl> controlesMudar = new List<WebControl>();
                                foreach (Control control in celula.Controls)
                                    if (control is WebControl)
                                        controlesMudar.Add((WebControl)control);
    
                                // Altera os controles
                                SetControlsParameters(benef, controlesMudar.ToArray());
                            }
                    }
                }
                else
                {
                    string onClick = "";
    
                    // Verifica se o controle é um ImageButton e se possui a propriedade OnClientClick com algum valor
                    if (c is ImageButton && !String.IsNullOrEmpty(((ImageButton)c).OnClientClick))
                    {
                        // Recupera o valor da propriedade
                        onClick = ((ImageButton)c).OnClientClick;
    
                        // Inclui a função no controle
                        if (onClick.ToLower().IndexOf("return") > -1)
                            onClick = onClick.Replace("return", GetFuncaoCalculo(benef) + "; return");
                        else
                            onClick += "; " + GetFuncaoCalculo(benef);
    
                        ((ImageButton)c).OnClientClick = onClick;
                    }
                    else
                    {
                        // Verifica se o controle já possui uma função OnClick
                        if (!String.IsNullOrEmpty(((WebControl)c).Attributes["OnClick"]))
                            onClick = ((WebControl)c).Attributes["OnClick"] + "; ";
    
                        ((WebControl)c).Attributes.Add("OnClick", onClick + GetFuncaoCalculo(benef));
                    }
                }
            }
        }
    
        /// <summary>
        /// Retorna o texto da função de cálculo JavaScript.
        /// Se um beneficiamento for passado o retorno será o cálculo para esse beneficiamento.
        /// Senão, será um cálculo para todos os beneficiamentos.
        /// </summary>
        /// <returns></returns>
        private string GetFuncaoCalculo(BenefConfig benef)
        {
            // Recupera o prefixo da tabela
            string prefixoTabela = this.ClientID + "_";
            
            // Se há um beneficiamento retorna a função para o cálculo dele
            if (benef != null)
            {
                string prefixoItem = prefixoTabela + PrefixoControles(benef);
                return "calculaValor('" + prefixoTabela + "', '" + prefixoItem + "', this, '" + _funcaoValorCalculoAdicional + "', " + 
                    _calcularValorAdicionalBenef.ToString().ToLower() + ", '" + _callbackCalculoValorItem + "', '" + 
                    _callbackCalculoValorTotal + "', '" + _callbackSelecaoItem + "')";
            }
    
            // Retorna a função para o cálculo de todos os beneficiamentos
            else
                return "calculaTodos('" + prefixoTabela + "')";
        }
    
        /// <summary>
        /// Retorna o texto da variável de configuração dos beneficiamentos.
        /// </summary>
        /// <param name="calculaveis">Os itens retornados devem ser os calculáveis?</param>
        /// <returns>Uma string com a variável para ser usada no JavaScript.</returns>
        private string GetConfig(bool calculaveis)
        {
            // Recupera a lista de beneficiamentos usados para gerar o retorno
            var benef = calculaveis ? BenefConfigDAO.Instance.GetForConfig() : BenefConfigDAO.Instance.GetForControl(TipoBenef);
    
            // String com o formato usado para o retorno
            string formato = "" +
                "ID: {0}, " +
                "TipoControle: {1}, " +
                "TipoCalculo: {2}, " +
                "ValorAtacado: {3}, " +
                "ValorBalcao: {4}, " +
                "ValorObra: {5}, " +
                "Custo: {6}, " +
                "ParentID: {7}, " +
                "Espessura: {8}, " +
                "Calcular: {9}, " +
                "TipoEspessura: {10}, " +
                "Descricao: '{11}', " +
                "SubgrupoID: {12}, " +
                "DescricaoParent: '{13}', " +
                "Cor: {14}, " +
                "AplicacaoID: {15}, " +
                "ProcessoID: {16}, " +
                "Aplicacao: '{17}', " +
                "Processo: '{18}'";
            
            if (!calculaveis)
                formato += ", CobrarAreaMinima: {19}";
    
            // Variável de retorno
            StringBuilder retorno = new StringBuilder();
            List<BenefConfigPreco> precos = new List<BenefConfigPreco>(BenefConfigPrecoDAO.Instance.GetByIdBenefConfig(0));
    
            // Percorre cada beneficiamento da lista
            foreach (BenefConfig b in benef)
            {
                // Variável com os dados usados para formatar a string
                object[] dadosFormato = new object[20];
    
                List<BenefConfigPreco> precosBenef = precos.FindAll(new Predicate<BenefConfigPreco>(
                    delegate(BenefConfigPreco p)
                    {
                        return p.IdBenefConfig == b.IdBenefConfig;
                    }
                ));
    
                if (calculaveis)
                {
                    foreach (BenefConfigPreco bp in precosBenef)
                    {
                        dadosFormato[0] = b.IdBenefConfig;
                        dadosFormato[1] = (int)b.TipoControle;
                        dadosFormato[2] = (int)b.TipoCalculo;
                        dadosFormato[3] = bp.ValorAtacado.ToString().Replace(',', '.');
                        dadosFormato[4] = bp.ValorBalcao.ToString().Replace(',', '.');
                        dadosFormato[5] = bp.ValorObra.ToString().Replace(',', '.');
                        dadosFormato[6] = bp.Custo.ToString().Replace(',', '.');
                        dadosFormato[7] = (b.IdParent != null ? b.IdParent.Value.ToString() : "null");
                        dadosFormato[8] = bp.Espessura > 0 ? bp.Espessura.Value : 0;
                        dadosFormato[9] = "true";
                        dadosFormato[10] = (int)b.TipoEspessura;
                        dadosFormato[11] = (b.Descricao != null ? b.Descricao : "");
                        dadosFormato[12] = (bp.IdSubgrupoProd != null ? bp.IdSubgrupoProd.Value.ToString() : "null");
                        dadosFormato[13] = b.DescricaoParent != null ? b.DescricaoParent + " " : "";
                        dadosFormato[14] = bp.IdCorVidro != null ? bp.IdCorVidro.Value.ToString() : "null";
                        dadosFormato[15] = b.IdAplicacao != null ? b.IdAplicacao.Value.ToString() : "null";
                        dadosFormato[16] = b.IdProcesso != null ? b.IdProcesso.Value.ToString() : "null";
                        dadosFormato[17] = b.CodAplicacao != null ? b.CodAplicacao.ToString() : "";
                        dadosFormato[18] = b.CodProcesso != null ? b.CodProcesso.ToString() : "";
                        dadosFormato[19] = b.CobrarAreaMinima.ToString().ToLower();
    
                        retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                    }
                }
                else
                {
                    dadosFormato[0] = b.IdBenefConfig;
                    dadosFormato[1] = (int)b.TipoControle;
                    dadosFormato[2] = (int)b.TipoCalculo;
                    dadosFormato[3] = "null";
                    dadosFormato[4] = "null";
                    dadosFormato[5] = "null";
                    dadosFormato[6] = "null";
                    dadosFormato[7] = (b.IdParent != null ? b.IdParent.Value.ToString() : "null");
                    dadosFormato[8] = "null";
                    dadosFormato[9] = "false";
                    dadosFormato[10] = (int)b.TipoEspessura;
                    dadosFormato[11] = (b.Descricao != null ? b.Descricao : "");
                    dadosFormato[12] = "null";
                    dadosFormato[13] = b.DescricaoParent != null ? b.DescricaoParent + " " : "";
                    dadosFormato[14] = "null";
                    dadosFormato[15] = b.IdAplicacao != null ? b.IdAplicacao.Value.ToString() : "null";
                    dadosFormato[16] = b.IdProcesso != null ? b.IdProcesso.Value.ToString() : "null";
                    dadosFormato[17] = b.CodAplicacao != null ? b.CodAplicacao.ToString() : "";
                    dadosFormato[18] = b.CodProcesso != null ? b.CodProcesso.ToString() : "";
                    dadosFormato[19] = b.CobrarAreaMinima.ToString().ToLower();
    
                    retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
                }
            }
    
            // Retorna a string com os dados dos beneficiamentos formatados
            return "new Array(" + (retorno.Length > 0 ? retorno.ToString().Substring(2) : "") + ")";
        }
    
        /// <summary>
        /// Retorna o texto da variável de beneficiamentos associados.
        /// </summary>
        /// <returns>Uma string com a variável para ser usada no JavaScript.</returns>
        private string GetConfigAssoc()
        {
            // Recupera a lista de beneficiamentos usados para gerar o retorno
            var benefAssoc = BenefConfigAssocDAO.Instance.GetAll();
    
            // String com o formato usado para o retorno
            string formato = "" +
                "ID: {0}, " +
                "AssocID: {1}, " +
                "AssocParentID: {2}, " +
                "CobrarAssoc: {3}, " +
                "BloquearAssoc: {4}, " +
                "TipoControleAssoc: {5}, " +
                "AlturaBenef: {6}, " +
                "LarguraBenef: {7}, " +
                "EspessuraBenef: {8}, " +
                "QtdeBenef: {9}, " +
                "PrefixoBenefAssoc: '{10}'";
    
            // Variável de retorno
            StringBuilder retorno = new StringBuilder();
    
            // Percorre cada beneficiamento da lista
            foreach (BenefConfigAssoc b in benefAssoc)
            {
                // Variável com os dados usados para formatar a string
                object[] dadosFormato = new object[11];
                dadosFormato[0] = b.IdBenefConfig;
                dadosFormato[1] = b.IdBenefConfigAssoc;
                dadosFormato[2] = b.IdParentAssoc != null ? b.IdParentAssoc.Value.ToString() : "null";
                dadosFormato[3] = b.CobrarAssoc.ToString().ToLower();
                dadosFormato[4] = b.BloquearAssoc.ToString().ToLower();
                dadosFormato[5] = b.TipoControleAssoc;
                dadosFormato[6] = b.AlturaBenef;
                dadosFormato[7] = b.LarguraBenef;
                dadosFormato[8] = b.EspessuraBenef.ToString().Replace(',', '.');
                dadosFormato[9] = b.QtdeBenef;
                dadosFormato[10] = PrefixoControles(b.IdParentAssoc != null ? BenefConfigDAO.Instance.GetElementByPrimaryKey(b.IdParentAssoc.Value) : 
                    BenefConfigDAO.Instance.GetElementByPrimaryKey(b.IdBenefConfigAssoc));
    
                retorno.Append(", { " + String.Format(formato, dadosFormato) + " }");
            }
    
            // Retorna a string com os dados dos beneficiamentos formatados
            return "new Array(" + (retorno.Length > 0 ? retorno.ToString().Substring(2) : "") + ")";
        }
    
        #endregion
    
        #region Geração e recuperação dos beneficiamentos
    
        #region Métodos de recuperação do identificador do beneficiamento
    
        /// <summary>
        /// Retorna o identificador do beneficiamento de um controle.
        /// </summary>
        /// <param name="controle">O controle que será pesquisado.</param>
        /// <returns></returns>
        private uint GetBenefID(WebControl controle)
        {
            string valor = controle.Attributes["idBeneficiamento"];
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaUint(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o identificador do beneficiamento aplicado da célula.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        private uint GetBenefID(string prefixo, TableCell celula)
        {
            string valor = GetControleIdBenefAplicado(prefixo, celula).Value;
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaUint(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o identificador do beneficiamento a partir do beneficiamento aplicado.
        /// </summary>
        /// <param name="benef">O beneficiamento aplicado.</param>
        /// <param name="typeID">O retorno deve ser do tipo do beneficiamento?</param>
        /// <returns>O identificador do beneficiamento.</returns>
        private uint GetBenefID(GenericBenef benef, bool typeID)
        {
            return BenefConfigDAO.Instance.GetTypeID(benef.IdBenefConfig);
            
            /*
            if (typeID)
                return BenefConfigDAO.Instance.GetTypeID(benef.IdBenefConfig);
            else
            {
                BenefConfig b = BenefConfigDAO.Instance.GetElementByPrimaryKey(benef.IdBenefConfig);
                if (b.IdParent != null)
                    b = b.FirstLevelParent;
    
                return b.IdBenefConfig;
            }
            */
        }
    
        #endregion
    
        #region Recuperação dos controles
    
        /// <summary>
        /// Recupera o controle de indicação da aplicação do beneficiamento.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private HiddenField GetControleIdBenefAplicado(string prefixo, TableCell celula)
        {
            return (HiddenField)celula.FindControl(prefixo + "hdfIdBenefAplicado");
        }
    
        /// <summary>
        /// Recupera o controle que indica se o beneficiamento é padrão.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private HiddenField GetControleBenefPadrao(string prefixo, TableCell celula)
        {
            return (HiddenField)celula.FindControl(prefixo + "hdfBenefPadrao");
        }
    
        /// <summary>
        /// Recupera o controle do valor unitário do beneficiamento.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private HiddenField GetControleValorUnit(string prefixo, TableCell celula, int posicao)
        {
            return (HiddenField)celula.FindControl(prefixo + "hdfValorUnit" + posicao);
        }
    
        /// <summary>
        /// Recupera o controle do valor do beneficiamento.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private HiddenField GetControleValor(string prefixo, TableCell celula, int posicao)
        {
            return (HiddenField)celula.FindControl(prefixo + "hdfValor" + posicao);
        }
    
        /// <summary>
        /// Recupera o controle do custo do beneficiamento.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private HiddenField GetControleCusto(string prefixo, TableCell celula, int posicao)
        {
            return (HiddenField)celula.FindControl(prefixo + "hdfCusto" + posicao);
        }
    
        /// <summary>
        /// Recupera o controle do tipo do beneficiamento.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private DropDownList GetControleTipo(string prefixo, TableCell celula)
        {
            return (DropDownList)celula.FindControl(prefixo + "drpTipo");
        }
    
        /// <summary>
        /// Retorna o controle de altura.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private DropDownList GetControleAltura(string prefixo, TableCell celula)
        {
            return (DropDownList)celula.FindControl(prefixo + "drpAltura");
        }
    
        /// <summary>
        /// Retorna o controle de largura.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private DropDownList GetControleLargura(string prefixo, TableCell celula)
        {
            return (DropDownList)celula.FindControl(prefixo + "drpLargura");
        }
    
        /// <summary>
        /// Retorna o controle de espessura do bisotê.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private TextBox GetControleEspessura(string prefixo, TableCell celula)
        {
            return (TextBox)celula.FindControl(prefixo + "txtEspessura");
        }
    
        /// <summary>
        /// Retorna o controle de quantidade.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private TextBox GetControleQuantidade(string prefixo, TableCell celula)
        {
            return (TextBox)celula.FindControl(prefixo + "tblQtd_txtQtd");
        }
    
        /// <summary>
        /// Retorna os controles de seleção múltipla.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private CheckBox[] GetControlesSelecaoMultipla(string prefixo, TableCell celula)
        {
            List<CheckBox> retorno = new List<CheckBox>();
            int i = 1;
            while (celula.FindControl(prefixo + "chkSelecao" + i) != null)
            {
                CheckBox chkSelecao = (CheckBox)celula.FindControl(prefixo + "chkSelecao" + i);
                i++;
    
                retorno.Add(chkSelecao);
            }
    
            return retorno.ToArray();
        }
    
        /// <summary>
        /// Retorna o controle de seleção simples.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private CheckBox GetControleSelecaoSimples(string prefixo, TableCell celula)
        {
            return (CheckBox)celula.FindControl(prefixo + "chkSelecao");
        }
    
        #endregion
    
        #region Geração
    
        /// <summary>
        /// Marca o CheckBox de cobrança opcional para o beneficiamento.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de bisotê.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarCobrancaOpcional(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            TableRow linha = celula.Parent as TableRow;
            TableCell cabecalho = linha.Cells[linha.Cells.GetCellIndex(celula) - 1];
    
            if (cabecalho.Attributes["Opcional"] == "true")
            {
                CheckBox chkOpcional = (CheckBox)cabecalho.FindControl(PrefixoControles(benef) + "chkOpcional");
                switch (benef.TipoControle)
                {
                    case TipoControleBenef.SelecaoMultiplaInclusiva:
                        chkOpcional.Checked = true;
                        break;
    
                    default:
                        chkOpcional.Checked = beneficiamentos[0].Valor > 0 || beneficiamentos[0].Custo > 0;
                        break;
                }
            }
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de bisotê.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de bisotê.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleBisote(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            DropDownList drpTipo = GetControleTipo(prefixo, celula);
            DropDownList drpAltura = GetControleAltura(prefixo, celula);
            DropDownList drpLargura = GetControleLargura(prefixo, celula);
            TextBox txtEspessura = GetControleEspessura(prefixo, celula);
    
            // Atribui os valores aos controles
            drpTipo.SelectedValue = GetBenefID(beneficiamentos[0], true).ToString();
            drpAltura.SelectedValue = beneficiamentos[0].BisAlt.ToString();
            drpLargura.SelectedValue = beneficiamentos[0].BisLarg.ToString();
            txtEspessura.Text = beneficiamentos[0].EspBisote.ToString();
    
            // Desabilita os controles se o beneficiamento for padrão
            drpTipo.Enabled = !beneficiamentos[0].Padrao;
            drpAltura.Enabled = !beneficiamentos[0].Padrao;
            drpLargura.Enabled = !beneficiamentos[0].Padrao;
            txtEspessura.Enabled = !beneficiamentos[0].Padrao;
        }
    
        //// <summary>
        /// Aplica os beneficiamentos aos controles de bisotê.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de bisotê.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleLapidacao(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            DropDownList drpTipo = GetControleTipo(prefixo, celula);
            DropDownList drpAltura = GetControleAltura(prefixo, celula);
            DropDownList drpLargura = GetControleLargura(prefixo, celula);
    
            // Atribui os valores aos controles
            drpTipo.SelectedValue = GetBenefID(beneficiamentos[0], true).ToString();
            drpAltura.SelectedValue = beneficiamentos[0].LapAlt.ToString();
            drpLargura.SelectedValue = beneficiamentos[0].LapLarg.ToString();
    
            // Desabilita os controles se o beneficiamento for padrão
            drpTipo.Enabled = !beneficiamentos[0].Padrao;
            drpAltura.Enabled = !beneficiamentos[0].Padrao;
            drpLargura.Enabled = !beneficiamentos[0].Padrao;
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de bisotê.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de bisotê.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleListaSelecao(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            DropDownList drpTipo = GetControleTipo(prefixo, celula);
    
            // Atribui os valores aos controles
            drpTipo.SelectedValue = GetBenefID(beneficiamentos[0], true).ToString();
    
            // Desabilita os controles se o beneficiamento for padrão
            drpTipo.Enabled = !beneficiamentos[0].Padrao;
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de bisotê.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de bisotê.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleListaSelecaoQtd(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            DropDownList drpTipo = GetControleTipo(prefixo, celula);
            TextBox txtQtd = GetControleQuantidade(prefixo, celula);
    
            // Atribui os valores aos controles
            drpTipo.SelectedValue = GetBenefID(beneficiamentos[0], true).ToString();
            txtQtd.Text = beneficiamentos[0].Qtd.ToString();
    
            // Desabilita os controles se o beneficiamento for padrão
            drpTipo.Enabled = !beneficiamentos[0].Padrao;
            txtQtd.Enabled = !beneficiamentos[0].Padrao;
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de quantidade.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de quantidade.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleQuantidade(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            TextBox txtQtd = GetControleQuantidade(prefixo, celula);
    
            // Atribui os valores aos controles
            txtQtd.Text = beneficiamentos[0].Qtd.ToString();
    
            // Desabilita os controles se o beneficiamento for padrão
            txtQtd.Enabled = !beneficiamentos[0].Padrao;
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de seleção múltipla exclusiva.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de seleção múltipla exclusiva.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleSelecaoMultiplaExclusiva(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Percorre os controles
            foreach (CheckBox chkSelecao in GetControlesSelecaoMultipla(prefixo, celula))
            {
                uint idBeneficiamento = GetBenefID(chkSelecao);
                foreach (GenericBenef b in beneficiamentos)
                    if (b.IdBenefConfig == idBeneficiamento)
                    {
                        // Atribui o valor ao controle
                        chkSelecao.Checked = true;
    
                        // Desabilita o controle se o beneficiamento for padrão
                        chkSelecao.Enabled = !b.Padrao;
                        return;
                    }
            }
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de seleção múltipla inclusiva.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de seleção múltipla inclusiva.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleSelecaoMultiplaInclusiva(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Percorre os controles
            foreach (CheckBox chkSelecao in GetControlesSelecaoMultipla(prefixo, celula))
            {
                uint idBeneficiamento = GetBenefID(chkSelecao);
                foreach (GenericBenef b in beneficiamentos)
                    if (b.IdBenefConfig == idBeneficiamento)
                    {
                        // Atribui o valor ao controle
                        chkSelecao.Checked = true;
    
                        // Desabilita o controle se o beneficiamento for padrão
                        break;
                    }
            }
        }
    
        /// <summary>
        /// Aplica os beneficiamentos aos controles de seleção simples.
        /// </summary>
        /// <param name="benef">O beneficiamento que a célula representa.</param>
        /// <param name="beneficiamentos">Os beneficiamentos que serão aplicados aos controles de seleção simples.</param>
        /// <param name="celula">A célula que contém os controles.</param>
        private void AplicarTipoControleSelecaoSimples(BenefConfig benef, GenericBenef[] beneficiamentos, TableCell celula)
        {
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Recupera os controles
            CheckBox chkSelecao = GetControleSelecaoSimples(prefixo, celula);
    
            // Atribui os valores aos controles
            chkSelecao.Checked = true;
    
            // Desabilita os controles se o beneficiamento for padrão
            chkSelecao.Enabled = !beneficiamentos[0].Padrao;
        }
    
        /// <summary>
        /// Define os beneficiamentos para uma célula da tabela do controle.
        /// </summary>
        /// <param name="beneficiamentos">Os beneficiamentos que serão usados pelo controle.</param>
        /// <param name="celula">A célula que receberá os beneficiamentos.</param>
        private void SetBeneficiamentosToCell(IList<GenericBenef> beneficiamentos, TableCell celula)
        {
            // Variável com o beneficiamento
            BenefConfig benef = null;
    
            try
            {
                // Recupera o beneficiamento da célula
                benef = BenefConfigDAO.Instance.GetElement(GetBenefID(celula));
            }
            catch
            {
                return;
            }
    
            // Procura o beneficiamento na lista
            GenericBenef item = beneficiamentos.FirstOrDefault(b =>
            {
                return b.IdBenefConfig == benef.IdBenefConfig;
    
            });
    
            // Variável com os beneficiamentos aplicados à célula
            GenericBenef[] dados = null;
    
            // Se o beneficiamento foi encontrado na lista indica-o nos dados
            if (item != null)
                dados = new GenericBenef[] { item };
    
            // Recupera os beneficiamentos filhos do beneficiamento da célula
            else
            {
                // Se o beneficiamento da célula não tem filhos sai do método
                if (benef.NumChild == 0)
                    return;
    
                // Recupera os filhos do beneficiamento da célula
                BenefConfig[] child = BenefConfigDAO.Instance.GetByBenefConfigItens((uint)benef.IdBenefConfig);
    
                // Procura entre os filhos o beneficiamento aplicado
                dados = beneficiamentos.Where(b =>
                {
                    foreach (BenefConfig bc in child)
                        if (b.IdBenefConfig == bc.IdBenefConfig)
                            return true;
    
                    return false;
    
                }).ToArray();
            }
    
            // Se o beneficiamento não foi encontrado sai do método
            if (dados.Length == 0)
                return;
    
            try
            {
                // Marca ou desmarca o CheckBox de cobrança opcional
                AplicarCobrancaOpcional(benef, dados, celula);
    
                // Aplica o beneficiamento de acordo com o tipo de controle
                switch (benef.TipoControle)
                {
                    case TipoControleBenef.Bisote:
                        AplicarTipoControleBisote(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.Lapidacao:
                        AplicarTipoControleLapidacao(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.ListaSelecao:
                        AplicarTipoControleListaSelecao(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.ListaSelecaoQtd:
                        AplicarTipoControleListaSelecaoQtd(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.Quantidade:
                        AplicarTipoControleQuantidade(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.SelecaoMultiplaExclusiva:
                        AplicarTipoControleSelecaoMultiplaExclusiva(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.SelecaoMultiplaInclusiva:
                        AplicarTipoControleSelecaoMultiplaInclusiva(benef, dados, celula);
                        break;
    
                    case TipoControleBenef.SelecaoSimples:
                        AplicarTipoControleSelecaoSimples(benef, dados, celula);
                        break;
                }
    
                // Indica à célula se o beneficiamento é padrão
                string prefixo = PrefixoControles(benef);
                GetControleBenefPadrao(prefixo, celula).Value = dados[0].Padrao.ToString().ToLower();
    
                // Exibe o valor do beneficiamento
                if (_exibirValorBeneficiamento)
                {
                    decimal valor = dados[0].ValorUnit;
    
                    // Calcula o valor unitário se este ainda não estiver salvo no banco de dados
                    if (valor == 0)
                    {
                        // Recupera os dados do produto
                        GenericBenef.DadosProduto dadosProduto = dados[0].GetProduto();
                        valor = dados[0].Valor;
    
                        // Calcula o valor unitário dependendo do tipo de cálculo
                        switch (benef.TipoCalculo)
                        {
                            case TipoCalculoBenef.MetroLinear:
                                int altura = 2;
                                int largura = 2;
    
                                if (benef.TipoControle == TipoControleBenef.Lapidacao || benef.TipoControle == TipoControleBenef.Bisote)
                                {
                                    altura = dados[0].LapAlt + dados[0].BisAlt;
                                    largura = dados[0].LapLarg + dados[0].BisLarg;
                                }
    
                                valor = valor / (decimal)(dadosProduto.Qtd * ((dadosProduto.Altura * altura) + (dadosProduto.Largura * largura)));
                                break;
    
                            case TipoCalculoBenef.MetroQuadrado:
                                valor = valor / (decimal)dadosProduto.TotalM2;
                                break;
    
                            case TipoCalculoBenef.Quantidade:
                                valor = valor / (decimal)(dados[0].Qtd * dadosProduto.Qtd);
                                break;
                        }
                    }
    
                    // Atualiza o valor no campo
                    TextBox campoValor = celula.FindControl(prefixo + "txtValorBenef") as TextBox;
                    if (campoValor != null)
                        campoValor.Text = valor.ToString("0.00");
                }
    
                // Verifica se o ID antigo será usado
                if (!String.IsNullOrEmpty(_propriedadeIdAntigo))
                {
                    try
                    {
                        // Altera o valor do campo que contém o ID antigo
                        HiddenField hidden = celula.FindControl(prefixo + "hdfIdAntigo") as HiddenField;
                        if (hidden != null)
                            hidden.Value = typeof(GenericBenef).GetProperty(_propriedadeIdAntigo).GetValue(dados[0], null).ToString();
                    }
                    catch { }
                }
            }
            catch { }
        }
    
        #endregion
    
        #region Recuperação
    
        /// <summary>
        /// Indica se o beneficiamento foi aplicado.
        /// </summary>
        /// <param name="celula"></param>
        /// <returns></returns>
        private bool IsBeneficiado(string prefixo, TableCell celula)
        {
            HiddenField campo = GetControleIdBenefAplicado(prefixo, celula);
            if (campo == null)
                return false;

            string valor = Page.Request.Form[campo.UniqueID] ?? campo.Value;
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaUint(valor) > 0 : false;
        }
    
        /// <summary>
        /// Retorna o valor do campo altura.
        /// </summary>
        /// <param name="celula"></param>
        /// <returns></returns>
        private int GetAltura(string prefixo, TableCell celula)
        {
            string valor = GetControleAltura(prefixo, celula).SelectedValue;
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaInt(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o valor do campo largura.
        /// </summary>
        /// <param name="celula"></param>
        /// <returns></returns>
        private int GetLargura(string prefixo, TableCell celula)
        {
            string valor = GetControleLargura(prefixo, celula).SelectedValue;
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaInt(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o valor do campo espessura.
        /// </summary>
        /// <param name="celula"></param>
        /// <returns></returns>
        private float GetEspessura(string prefixo, TableCell celula)
        {
            string valor = GetControleEspessura(prefixo, celula).Text;
            return !String.IsNullOrEmpty(valor) ? float.Parse(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o valor do campo quantidade.
        /// </summary>
        /// <param name="celula"></param>
        /// <returns></returns>
        private int GetQuantidade(string prefixo, TableCell celula)
        {
            string valor = GetControleQuantidade(prefixo, celula).Text;
            return !String.IsNullOrEmpty(valor) ? Glass.Conversoes.StrParaInt(valor) : 0;
        }
    
        /// <summary>
        /// Retorna o valor do campo valor unitário.
        /// </summary>
        /// <param name="celula"></param>
        /// <param name="posicao"></param>
        /// <returns></returns>
        private decimal GetValorUnit(string prefixo, TableCell celula, int posicao)
        {
            string valor = GetControleValorUnit(prefixo, celula, posicao).Value;
            return Glass.Conversoes.StrParaDecimal(valor);
        }
    
        /// <summary>
        /// Retorna o valor do campo valor.
        /// </summary>
        /// <param name="celula"></param>
        /// <param name="posicao"></param>
        /// <returns></returns>
        private decimal GetValor(string prefixo, TableCell celula, int posicao)
        {
            string valor = GetControleValor(prefixo, celula, posicao).Value;
            return Glass.Conversoes.StrParaDecimal(valor);
        }
    
        /// <summary>
        /// Retorna o valor do campo custo.
        /// </summary>
        /// <param name="celula"></param>
        /// <param name="posicao"></param>
        /// <returns></returns>
        private decimal GetCusto(string prefixo, TableCell celula, int posicao)
        {
            string valor = GetControleCusto(prefixo, celula, posicao).Value;
            return Glass.Conversoes.StrParaDecimal(valor);
        }
    
        /// <summary>
        /// Indica se o beneficiamento é padrão.
        /// </summary>
        /// <param name="prefixo"></param>
        /// <param name="celula"></param>
        /// <returns></returns>
        private bool GetPadrao(string prefixo, TableCell celula)
        {
            string valor = GetControleBenefPadrao(prefixo, celula).Value;
            return !String.IsNullOrEmpty(valor) ? bool.Parse(valor) : false;
        }
    
        /// <summary>
        /// Retorna os beneficiamentos feitos em uma célula.
        /// </summary>
        /// <param name="celula">A célula com os controles do beneficiamento.</param>
        /// <returns>Um vetor com os beneficiamentos feitos</returns>
        private GenericBenef[] GetBeneficiamentosFromCell(TableCell celula)
        {
            // Recupera o beneficiamento da célula
            BenefConfig benef = BenefConfigDAO.Instance.GetElement(GetBenefID(celula));
    
            // Cria o prefixo
            string prefixo = PrefixoControles(benef);
    
            // Cria a lista de retorno
            List<GenericBenef> retorno = new List<GenericBenef>();
    
            // Verifica se a célula possui beneficiamento
            if (IsBeneficiado(prefixo, celula))
            {
                // Recupera os beneficiamentos de acordo com o tipo de controle
                switch (benef.TipoControle)
                {
                    case TipoControleBenef.Bisote:
                        retorno.Add(new GenericBenef());
                        retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                        retorno[0].BisAlt = GetAltura(prefixo, celula);
                        retorno[0].BisLarg = GetLargura(prefixo, celula);
                        retorno[0].EspBisote = GetEspessura(prefixo, celula);
                        retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                        retorno[0].Valor = GetValor(prefixo, celula, 1);
                        retorno[0].Custo = GetCusto(prefixo, celula, 1);
                        retorno[0].Padrao = GetPadrao(prefixo, celula);
                        break;
    
                    case TipoControleBenef.Lapidacao:
                        retorno.Add(new GenericBenef());
                        retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                        retorno[0].LapAlt = GetAltura(prefixo, celula);
                        retorno[0].LapLarg = GetLargura(prefixo, celula);
                        retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                        retorno[0].Valor = GetValor(prefixo, celula, 1);
                        retorno[0].Custo = GetCusto(prefixo, celula, 1);
                        retorno[0].Padrao = GetPadrao(prefixo, celula);
                        break;
    
                    case TipoControleBenef.ListaSelecao:
                        retorno.Add(new GenericBenef());
                        retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                        retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                        retorno[0].Valor = GetValor(prefixo, celula, 1);
                        retorno[0].Custo = GetCusto(prefixo, celula, 1);
                        retorno[0].Padrao = GetPadrao(prefixo, celula);
                        break;
    
                    case TipoControleBenef.ListaSelecaoQtd:
                        retorno.Add(new GenericBenef());
                        retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                        retorno[0].Qtd = GetQuantidade(prefixo, celula);
                        retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                        retorno[0].Valor = GetValor(prefixo, celula, 1);
                        retorno[0].Custo = GetCusto(prefixo, celula, 1);
                        retorno[0].Padrao = GetPadrao(prefixo, celula);
                        break;
    
                    case TipoControleBenef.Quantidade:
                        retorno.Add(new GenericBenef());
                        retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                        retorno[0].Qtd = GetQuantidade(prefixo, celula);
                        retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                        retorno[0].Valor = GetValor(prefixo, celula, 1);
                        retorno[0].Custo = GetCusto(prefixo, celula, 1);
                        retorno[0].Padrao = GetPadrao(prefixo, celula);
                        break;
    
                    case TipoControleBenef.SelecaoMultiplaExclusiva:
                        foreach (CheckBox chkSelecaoM in GetControlesSelecaoMultipla(prefixo, celula))
                            if (chkSelecaoM.Checked)
                            {
                                retorno.Add(new GenericBenef());
                                retorno[0].IdBenefConfig = GetBenefID(chkSelecaoM);
                                retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                                retorno[0].Valor = GetValor(prefixo, celula, 1);
                                retorno[0].Custo = GetCusto(prefixo, celula, 1);
                                retorno[0].Padrao = GetPadrao(prefixo, celula);
                                break;
                            }
                        break;
    
                    case TipoControleBenef.SelecaoMultiplaInclusiva:
                        foreach (CheckBox chkSelecaoM in GetControlesSelecaoMultipla(prefixo, celula))
                        {
                            int i = Glass.Conversoes.StrParaInt(chkSelecaoM.ID.Substring(chkSelecaoM.ID.IndexOf("_chkSelecao") + 11));
                            if (chkSelecaoM.Checked)
                            {
                                retorno.Add(new GenericBenef());
                                retorno[retorno.Count - 1].IdBenefConfig = GetBenefID(chkSelecaoM);
                                retorno[retorno.Count - 1].ValorUnit = GetValorUnit(prefixo, celula, i);
                                retorno[retorno.Count - 1].Valor = GetValor(prefixo, celula, i);
                                retorno[retorno.Count - 1].Custo = GetCusto(prefixo, celula, i);
                                retorno[retorno.Count - 1].Padrao = GetPadrao(prefixo, celula);
                            }
                        }
                        break;
    
                    case TipoControleBenef.SelecaoSimples:
                        CheckBox chkSelecao = GetControleSelecaoSimples(prefixo, celula);
                        if (chkSelecao.Checked)
                        {
                            retorno.Add(new GenericBenef());
                            retorno[0].IdBenefConfig = GetBenefID(prefixo, celula);
                            retorno[0].ValorUnit = GetValorUnit(prefixo, celula, 1);
                            retorno[0].Valor = GetValor(prefixo, celula, 1);
                            retorno[0].Custo = GetCusto(prefixo, celula, 1);
                            retorno[0].Padrao = GetPadrao(prefixo, celula);
                        }
                        break;
                }
    
                if (!String.IsNullOrEmpty(_propriedadeIdAntigo))
                {
                    try
                    {
                        // Recupera o valor do campo que contém o ID antigo
                        HiddenField hidden = celula.FindControl(PrefixoControles(benef) + "hdfIdAntigo") as HiddenField;
                        if (hidden != null)
                        {
                            PropertyInfo propriedade = typeof(GenericBenef).GetProperty(_propriedadeIdAntigo);
                            object valor = Convert.ChangeType(hidden.Value, propriedade.PropertyType);
                            propriedade.SetValue(retorno[0], valor, null);
                        }
                    }
                    catch { }
                }
            }
    
            // Retorna os beneficiamentos
            return retorno.ToArray();
        }
    
        #endregion
    
        #endregion
    
        #region Métodos Ajax
    
        /// <summary>
        /// Retorna o identificador do grupo do produto.
        /// </summary>
        /// <param name="idClienteStr">O id do cliente.</param>
        /// <param name="codInterno">O código interno do produto.</param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetDadosProduto(string idClienteStr, string codInterno)
        {
            // Recupera o produto pelo ID
            Produto p = !String.IsNullOrEmpty(codInterno) ? ProdutoDAO.Instance.GetByCodInterno(codInterno) : new Produto();

            if (p == null)
                p = new Produto();

            // Converte o ID do cliente para UInt
            uint idCliente = !String.IsNullOrEmpty(idClienteStr) ? Glass.Conversoes.StrParaUint(idClienteStr) : 0;
    
            // Formato da string de retorno
            string formato = "" +
                "ID: {0}, " +
                "Grupo: {1}, " +
                "Subgrupo: {2}, " +
                "Custo: {3}, " +
                "TipoCalculo: {4}, " +
                "CodInterno: '{5}', " +
                "DescontoAcrescimo: {6}, " +
                "UsarDescontoAcrescimo: {7}, " +
                "Cor: {8}, " +
                "IsSubgrupoEstoque: {9}, " +
                "IsChapaVidro: {10}";

            // Recupera o desconto/acréscimo do cliente
            DescontoAcrescimoCliente desconto = idCliente > 0 ? 
                DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(idCliente, p.IdGrupoProd, p.IdSubgrupoProd, p.IdProd, null, null) : null;
    
            // Vetor com os dados usados na formatação da string
            object[] dadosFormato = new object[11];
            dadosFormato[0] = p.IdProd;
            dadosFormato[1] = p.IdGrupoProd;
            dadosFormato[2] = p.IdSubgrupoProd != null ? p.IdSubgrupoProd.Value.ToString() : "null";
            dadosFormato[3] = p.CustoCompra.ToString().Replace(",", ".");
            dadosFormato[4] = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(p.IdGrupoProd, p.IdSubgrupoProd);
            dadosFormato[5] = p.CodInterno;
            dadosFormato[6] = idCliente > 0 ? desconto.PercMultiplicar.ToString().Replace(',', '.') : "0";
            dadosFormato[7] = idCliente > 0 ? desconto.AplicarBeneficiamentos.ToString().ToLower() : "0";
            dadosFormato[8] = p.IdCorVidro != null ? p.IdCorVidro.Value.ToString() : "null";
            dadosFormato[9] = SubgrupoProdDAO.Instance.IsSubgrupoProducao(p.IdGrupoProd, p.IdSubgrupoProd).ToString().ToLower();
            dadosFormato[10] = (p.IdSubgrupoProd > 0 ? SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(p.IdProd) == TipoSubgrupoProd.ChapasVidro : false).ToString().ToLower();

            // Retorna os dados do produto
            return "{ " + String.Format(formato, dadosFormato) + " }";
        }
    
        [Ajax.AjaxMethod]
        public string GetDadosPecaItemProjeto(string idPecaItemProjeto)
        {
            // Formato da string de retorno
            string formato = "" +
                "ID: {0}";
    
            // Vetor com os dados usados na formatação da string
            object[] dadosFormato = new object[1];
            dadosFormato[0] = idPecaItemProjeto;
            
            // Retorna os dados da peça
            return "{ " + String.Format(formato, dadosFormato) + " }";
        }

        /// <summary>
        /// Retorna custo do beneficiamento.
        /// </summary>
        /// <param name="idBenef">O id do beneficiamento.</param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetCustoBenef(string idBenef)
        {
            // Converte o ID do beneficiamento para UInt.
            uint idBenefConfig = !String.IsNullOrEmpty(idBenef) ? Glass.Conversoes.StrParaUint(idBenef) : 0;
            // Retorna o valor de custo do beneficiamento.
            return idBenefConfig > 0 ? BenefConfigPrecoDAO.Instance.ObtemCustoBenef(null, idBenefConfig, 0).ToString() : "0";
        }
    
        /// <summary>
        /// Retorna os beneficiamentos básicos de um produto.
        /// </summary>
        /// <param name="idProdStr">O id do produto.</param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string GetBenefByProd(string idProdStr, string tipo)
        {
            // Converte o ID do produto para UInt
            uint idProd = !String.IsNullOrEmpty(idProdStr) ? Glass.Conversoes.StrParaUint(idProdStr) : 0;
    
            // Formato da string de retorno
            string formato = "" +
                "ID: {0}, " +
                "ParentID: {1}, " +
                "Altura: {2}, " +
                "Largura: {3}, " +
                "Espessura: {4}, " +
                "Qtde: {5}, " +
                "TipoControle: {6}, " +
                "Prefixo: '{7}'";
    
            string retorno = "";
    
            // Variável de controle
            GenericBenefCollection beneficiamentos = new GenericBenefCollection();
    
            switch (tipo.ToLower())
            {
                case "produto":
                    beneficiamentos = ProdutoBenefDAO.Instance.GetByProduto(idProd);
                    break;
    
                case "projeto":
                    beneficiamentos = PecaItemProjBenefDAO.Instance.GetByPecaItemProj(idProd);
                    break;
    
                case "orçamento":
                    beneficiamentos = ProdutoOrcamentoBenefDAO.Instance.GetByProdutoOrcamento(idProd);
                    break;
    
                case "material":
                    beneficiamentos = MaterialProjetoBenefDAO.Instance.GetByMaterial(idProd);
                    break;
            }
    
            // Recupera todos os beneficiamentos do produto
            foreach (GenericBenef b in beneficiamentos)
            {
                // Recupera os beneficiamentos
                BenefConfig benef = BenefConfigDAO.Instance.GetElementByPrimaryKey(b.IdBenefConfig);
                while (benef.TipoEspessura == TipoEspessuraBenef.ItemEEspessura)
                {
                    if (benef.IdParent != null)
                        benef = BenefConfigDAO.Instance.GetElementByPrimaryKey((uint)benef.IdParent.Value);
                    else
                        break;
                }
    
                if (benef.TipoEspessura == TipoEspessuraBenef.ItemEEspessura)
                    continue;

                BenefConfig parent = benef.IdParent != null ? BenefConfigDAO.Instance.GetElementByPrimaryKey((uint)benef.IdParent.Value) : null;
    
                // ANDRÉ: O tipo controle ficando vazio, não recalculava corretamente os beneficiamentos, 
                // na opção recalcular orçamento.
                if (benef.TipoControle == 0 && parent != null)
                    benef.TipoControle = parent.TipoControle;
    
                // Vetor com os dados usados na formatação da string
                object[] dadosFormato = new object[8];
                dadosFormato[0] = benef.IdBenefConfig;
                dadosFormato[1] = benef.IdParent != null ? benef.IdParent.ToString() : "null";
                dadosFormato[2] = benef.TipoControle == TipoControleBenef.Lapidacao ? b.LapAlt :
                    benef.TipoControle == TipoControleBenef.Bisote ? b.BisAlt : 0;
                dadosFormato[3] = benef.TipoControle == TipoControleBenef.Lapidacao ? b.LapLarg :
                    benef.TipoControle == TipoControleBenef.Bisote ? b.BisLarg : 0;
                dadosFormato[4] = benef.TipoControle == TipoControleBenef.Bisote ? b.EspBisote.ToString().Replace(",", ".") : "0";
                dadosFormato[5] = b.Qtd;
                dadosFormato[6] = (int)benef.TipoControle;
                dadosFormato[7] = PrefixoControles(parent != null ? parent : benef);
    
                // Adiciona o beneficiamento à string de retorno
                retorno += ", {" + String.Format(formato, dadosFormato) + "}";
            }
    
            // Retorna os beneficiamentos
            return retorno.Length > 0 ? "new Array(" + retorno.Substring(1) + ");" : "null";
        }
    
        /// <summary>
        /// Verifica se um vidro é redondo.
        /// </summary>
        /// <param name="idProdStr">O id do produto.</param>
        /// <returns></returns>
        [Ajax.AjaxMethod]
        public string IsRedondo(string idProdStr, string tipo)
        {
            // Converte o ID do produto para UInt
            uint idProd = !String.IsNullOrEmpty(idProdStr) ? Glass.Conversoes.StrParaUint(idProdStr) : 0;
    
            // Variável de retorno
            bool redondo = false;
    
            // Retorna se o produto é redondo
            switch (tipo.ToLower())
            {
                case "produto": 
                    redondo = ProdutoDAO.Instance.IsRedondo(idProd);
                    break;
    
                case "projeto":
                    redondo = PecaItemProjetoDAO.Instance.IsRedondo(idProd);
                    break;
    
                case "orçamento": 
                    redondo = ProdutosOrcamentoDAO.Instance.IsRedondo(idProd);
                    break;
    
                case "material":
                    redondo = MaterialItemProjetoDAO.Instance.IsRedondo(idProd);
                    break;
            }
    
            return redondo.ToString().ToLower();
        }
    
        #endregion
    
        #region Propriedades
    
        /// <summary>
        /// Define o grupo de validação do controle.
        /// </summary>
        public string ValidationGroup
        {
            get { return vsuSumario.ValidationGroup; }
            set { vsuSumario.ValidationGroup = value; }
        }
    
        /// <summary>
        /// Os beneficiamentos padrão do produto devem ser carregados pelo controle?
        /// </summary>
        public bool CarregarBenefPadrao
        {
            get { return _carregarBenefPadrao; }
            set { _carregarBenefPadrao = value; }
        }
    
        /// <summary>
        /// O controle será usado na compra?
        /// </summary>
        public bool IsCompra
        {
            get { return _isCompra; }
            set { _isCompra = value; }
        }
    
        /// <summary>
        /// Mensagem que será exibida se os beneficiamentos estiverem bloqueados.
        /// </summary>
        public string MensagemBloqueioBenef
        {
            get { return _mensagemBloqueioBenef; }
            set { _mensagemBloqueioBenef = value; }
        }
    
        /// <summary>
        /// Nome da propriedade que contém o ID do beneficiamento antigo.
        /// Usado na tela de compras.
        /// </summary>
        public string PropriedadeIdAntigo
        {
            get { return _propriedadeIdAntigo; }
            set { _propriedadeIdAntigo = value; }
        }
    
        /// <summary>
        /// O campo com o valor do beneficiamento deve ser exibido?
        /// </summary>
        public bool ExibirValorBeneficiamento
        {
            get { return _exibirValorBeneficiamento; }
            set { _exibirValorBeneficiamento = value; }
        }
    
        /// <summary>
        /// Define se os beneficiamentos serão bloqueados para alteração.
        /// </summary>
        public bool BloquearBeneficiamentos
        {
            get { return _bloquearBeneficiamentos; }
            set
            {
                _bloquearBeneficiamentos = value;
    
                // Percorre as linhas da tabela
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Bloqueia/libera os controles da primeira célula
                    foreach (Control c in tblBenef.Rows[i].Cells[1].Controls)
                    {
                        if (!(c is WebControl))
                            continue;
    
                        ((WebControl)c).Enabled = !value;
                    }
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;
    
                    // Bloqueia/libera os controles da segunda célula
                    foreach (Control c in tblBenef.Rows[i].Cells[1].Controls)
                    {
                        if (!(c is WebControl))
                            continue;
    
                        ((WebControl)c).Enabled = !value;
                    }
                }
            }
        }
    
        /// <summary>
        /// Nome da função usada para receber valor adicional de cálculo de beneficiamento.
        /// </summary>
        public string FuncaoCalculoValorAdicional
        {
            get { return _funcaoValorCalculoAdicional; }
            set { _funcaoValorCalculoAdicional = value; }
        }
    
        /// <summary>
        /// O valor adicional deve ser calculado ao calcular o beneficiamento?
        /// (Se falso será aplicado após o cálculo.)
        /// </summary>
        public bool CalcularValorAdicionalBenef
        {
            get { return _calcularValorAdicionalBenef; }
            set { _calcularValorAdicionalBenef = value; }
        }
    
        /// <summary>
        /// Callback chamado quando o item for selecionado.
        /// </summary>
        public string CallbackSelecaoItem
        {
            get { return _callbackSelecaoItem; }
            set { _callbackSelecaoItem = value; }
        }
    
        /// <summary>
        /// Callback chamado quando o valor do item for atualizado.
        /// </summary>
        public string CallbackCalculoValorItem
        {
            get { return _callbackCalculoValorItem; }
            set { _callbackCalculoValorItem = value; }
        }
    
        /// <summary>
        /// Callback chamado quando o valor total for atualizado.
        /// </summary>
        public string CallbackCalculoValorTotal
        {
            get { return _callbackCalculoValorTotal; }
            set { _callbackCalculoValorTotal = value; }
        }
    
        /// <summary>
        /// Classe CSS do cabeçalho.
        /// </summary>
        public string CssClassCabecalho
        {
            get { return _cssClassCabecalho; }
            set { _cssClassCabecalho = value; }
        }
    
        /// <summary>
        /// Classe CSS da linha.
        /// </summary>
        public string CssClassLinha
        {
            get { return _cssClassLinha; }
            set { _cssClassLinha = value; }
        }
    
        /// <summary>
        /// Classe CSS da linha alternada.
        /// </summary>
        public string CssClassLinhaAlternada
        {
            get { return _cssClassLinhaAlternada; }
            set { _cssClassLinhaAlternada = value; }
        }

        /// <summary>
        /// Nome do tipo da classe do beneficiamento.
        /// </summary>
        public string TipoBeneficiamento
        {
            get { return _tipoBeneficiamento; }
            set { _tipoBeneficiamento = value; }
        }

        /// <summary>
        /// Beneficiamentos associados.
        /// </summary>
        public IEnumerable<Glass.Global.Negocios.Entidades.IBeneficiamento> Beneficiamentos2
        {
            get { return _beneficiamentos2; }
            set { _beneficiamentos2 = value; }
        }

        /// <summary>
        /// Os beneficiamentos feitos.
        /// </summary>
        public GenericBenefCollection Beneficiamentos
        {
            get
            {
                // Cria a variável de retorno
                GenericBenefCollection retorno = new GenericBenefCollection();
    
                // Percorre as linhas da tabela
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui no retorno os beneficiamentos da primeira célula
                    foreach (var beneficiamento in GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[1]))
                        retorno.Add(beneficiamento);
                    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;

                    // Inclui no retorno os beneficiamentos da segunda célula
                    foreach (var beneficiamento in GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[3]))
                        retorno.Add(beneficiamento);
                }
    
                return retorno;
            }
            set
            {
                // Percorre as linhas da coluna
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui na primeira célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[1]);
    
                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;
    
                    // Inclui na segunda célula os beneficiamentos
                    SetBeneficiamentosToCell(value, tblBenef.Rows[i].Cells[3]);
                }
            }
        }
    
        /// <summary>
        /// Retorna o valor total dos beneficiamentos.
        /// </summary>
        public decimal ValorTotal
        {
            get { return decimal.Parse(hdfValorTotal.Value.Replace('.', ',')); }
        }
    
        /// <summary>
        /// Retorna o custo total dos beneficiamentos.
        /// </summary>
        public decimal CustoTotal
        {
            get { return decimal.Parse(hdfCustoTotal.Value.Replace('.', ',')); }
        }
    
        /// <summary>
        /// O controle de 'Redondo' deve ser marcado?
        /// </summary>
        public bool Redondo
        {
            get
            {
                CheckBox chkRedondo = GetControlRedondo();
                return chkRedondo != null ? chkRedondo.Checked : false;
            }
            set
            {
                CheckBox chkRedondo = GetControlRedondo();
                if (chkRedondo != null)
                    chkRedondo.Checked = value;
            }
        }
    
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set
            {
                // Atualiza o ViewState do controle
                base.EnableViewState = value;
    
                // Atualiza os ViewStates dos controles filhos
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                    for (int j = 0; j < tblBenef.Rows[i].Cells.Count; j++)
                        foreach (Control c in tblBenef.Rows[i].Cells[j].Controls)
                            c.EnableViewState = value;
            }
        }
    
        public bool ValidarEspessura
        {
            get { return ctvEspessura.Enabled; }
            set { ctvEspessura.Enabled = value; }
        }
    
        public bool SomarTotalValorBenef
        {
            get { return _somarTotalValorBenef; }
            set { _somarTotalValorBenef = value; }
        }
    
        public bool CalcularBeneficiamentoPadrao
        {
            get { return _calcularBeneficiamentoPadrao; }
            set { _calcularBeneficiamentoPadrao = value; }
        }
    
        public TipoBenef TipoBenef
        {
            get { return _tipoBenef; }
            set { _tipoBenef = value; }
        }

        /// <summary>
        /// IdProdPed do pai do produto da composição
        /// </summary>
        public object IdProdPed
        {
            get
            {
                return hdf_benef_IdProdPed.Value;
            }
            set
            {
                hdf_benef_IdProdPed.Value = value.ToString();
            }
        }

        #endregion

        #region Propriedades de controles da página

        /// <summary>
        /// Campo com o ID da peça item projeto.
        /// </summary>
        public Control CampoPecaItemProjetoID
        {
            get { return _campoIdPecaItemProjeto; }
            set { _campoIdPecaItemProjeto = value; }
        }
    
        /// <summary>
        /// Campo com o ID do produto.
        /// </summary>
        public Control CampoProdutoID
        {
            get { return _campoIdProduto; }
            set { _campoIdProduto = value; }
        }
    
        /// <summary>
        /// Campo com a espessura do vidro.
        /// </summary>
        public Control CampoEspessura
        {
            get { return _campoEspessura; }
            set { _campoEspessura = value; }
        }
    
        /// <summary>
        /// Campo com o tipo de entrega.
        /// </summary>
        public Control CampoTipoEntrega
        {
            get { return _campoTipoEntrega; }
            set { _campoTipoEntrega = value; }
        }
    
        /// <summary>
        /// Campo com o percentual de comissão.
        /// </summary>
        public Control CampoPercComissao
        {
            get { return _campoPercComissao; }
            set { _campoPercComissao = value; }
        }
    
        /// <summary>
        /// Campo com a altura do vidro.
        /// </summary>
        public Control CampoAltura
        {
            get { return _campoAltura; }
            set { _campoAltura = value; }
        }
    
        /// <summary>
        /// Campo com a largura do vidro.
        /// </summary>
        public Control CampoLargura
        {
            get { return _campoLargura; }
            set { _campoLargura = value; }
        }
    
        /// <summary>
        /// Campo com o total de m² do vidro.
        /// </summary>
        public Control CampoTotalM2
        {
            get { return _campoTotalM2; }
            set { _campoTotalM2 = value; }
        }
    
        /// <summary>
        /// Campo com o valor unitário do vidro.
        /// </summary>
        public Control CampoValorUnitario
        {
            get { return _campoValorUnitario; }
            set { _campoValorUnitario = value; }
        }
    
        /// <summary>
        /// Campo com o custo do vidro.
        /// </summary>
        public Control CampoCusto
        {
            get { return _campoCusto; }
            set { _campoCusto = value; }
        }
    
        /// <summary>
        /// Campo com a quantidade de vidros.
        /// </summary>
        public Control CampoQuantidade
        {
            get { return _campoQuantidade; }
            set { _campoQuantidade = value; }
        }

        /// <summary>
        /// Campo com a quantidade de ambientes.
        /// </summary>
        public Control CampoQuantidadeAmbiente
        {
            get { return _campoQuantidadeAmbiente; }
            set { _campoQuantidadeAmbiente = value; }
        }
    
        /// <summary>
        /// Campo com a indicação de cliente de revenda.
        /// </summary>
        public Control CampoRevenda
        {
            get { return _campoRevenda; }
            set { _campoRevenda = value; }
        }
    
        /// <summary>
        /// Campo com o ID do cliente.
        /// </summary>
        public Control CampoClienteID
        {
            get { return _campoIdCliente; }
            set { _campoIdCliente = value; }
        }
    
        /// <summary>
        /// Campo com o ID da aplicação.
        /// </summary>
        public Control CampoAplicacaoID
        {
            get { return _campoIdAplicacao; }
            set { _campoIdAplicacao = value; }
        }
    
        /// <summary>
        /// Campo com o ID do processo.
        /// </summary>
        public Control CampoProcessoID
        {
            get { return _campoIdProcesso; }
            set { _campoIdProcesso = value; }
        }
    
        /// <summary>
        /// Campo com o código da aplicação.
        /// </summary>
        public Control CampoAplicacao
        {
            get { return _campoAplicacao; }
            set { _campoAplicacao = value; }
        }
    
        /// <summary>
        /// Campo com o código do processo.
        /// </summary>
        public Control CampoProcesso
        {
            get { return _campoProcesso; }
            set { _campoProcesso = value; }
        }
    
        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria um novo beneficiamento.
        /// </summary>
        /// <returns></returns>
        private Glass.Global.Negocios.Entidades.IBeneficiamento CriarBeneficiamento()
        {
            if (string.IsNullOrEmpty(TipoBeneficiamento))
                throw new InvalidOperationException(string.Format("Não foi informado o tipo do beneficiamento no controle {0}", ID));

            Type type = System.Web.Compilation.BuildManager.GetType(TipoBeneficiamento, false, true);

            if (type == null)
                throw new InvalidOperationException(string.Format("O tipo {0} não foi encontrado para criar o beneficiamento no controle {1}.", TipoBeneficiamento, ID));

            try
            {
                return Activator.CreateInstance(type) as Glass.Global.Negocios.Entidades.IBeneficiamento;
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Converte o GenericBenef para IBeneficiamento.
        /// </summary>
        /// <param name="beneficiamentos"></param>
        /// <returns></returns>
        public IEnumerable<Glass.Global.Negocios.Entidades.IBeneficiamento> Converter(IEnumerable<GenericBenef> beneficiamentos)
        {
            foreach (var i in beneficiamentos)
            {
                var item = CriarBeneficiamento();

                item.BisAlt = i.BisAlt;
                item.BisLarg = i.BisLarg;
                item.Custo = i.Custo;
                item.EspBisote = i.EspBisote;
                item.EspFuro = i.EspFuro;
                item.IdBenefConfig = (int)i.IdBenefConfig;
                item.LapAlt = i.LapAlt;
                item.LapLarg = i.LapLarg;
                item.Padrao = i.Padrao;
                item.Qtd = i.Qtd;
                item.Valor = i.Valor;
                item.ValorAcrescimo = i.ValorAcrescimo;
                item.ValorAcrescimoProd = i.ValorAcrescimoProd;
                item.ValorComissao = i.ValorComissao;
                item.ValorDesconto = i.ValorDesconto;
                item.ValorDescontoProd = i.ValorDescontoProd;
                item.ValorUnit = i.ValorUnit;

                yield return item;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Cria a variável que contém os itens vazios dos DropDownLists de seleção de tipo
            itensNenhum = new List<KeyValuePair<int, KeyValuePair<int, int>>>();

            // Registra os scripts
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("ctrlBenef"))
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlBenef));
                Page.ClientScript.RegisterClientScriptInclude("ctrlBenef", ResolveClientUrl("~/Scripts/ctrlBenef.js?v=" + Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterClientScriptInclude("ctrlBenef_CallBackSelecao", ResolveClientUrl("~/Scripts/CallbackItem_ctrlBenef.js?v=" + Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterClientScriptInclude("ctrlBenef_Aluminio", ResolveClientUrl("~/Scripts/CalcAluminio.js?v=" + Geral.ObtemVersao(true)));
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlBenef_Config", "var benefConfig = " + GetConfig(true) + ";\n", true);
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlBenef_ConfigAssoc", "var benefConfigAssoc = " + GetConfigAssoc() + ";\n", true);
                Page.ClientScript.RegisterOnSubmitStatement(GetType(), "ctrlBenef_Habilitar", "benefSubmit();\n");
                Page.ClientScript.RegisterStartupScript(GetType(), "ctrlBenef_HabilitarVariavel", "var benef_habilitar = new Array();\n", true);
            }

            Page.ClientScript.RegisterStartupScript(GetType(), "ctrlBenef_Habilitar_" + this.ID, "benef_habilitar.push('" + this.ClientID + "');\n", true);

            // Indica o evento de PreRender da página
            this.PreRender += new EventHandler(ctrlBenef_PreRender);

            // Define as funções JavaScript de callback para cálculo
            this.CallbackSelecaoItem = "callbackSelecao";
            this.FuncaoCalculoValorAdicional = Glass.Configuracoes.Beneficiamentos.ControleBeneficiamento.NomeFuncaoJavascriptCalculoValorAdicional;
            this.CalcularValorAdicionalBenef = !string.IsNullOrEmpty(this.FuncaoCalculoValorAdicional);

            // Recupera a lista de beneficiamentos e define o número de linhas
            var lstBenef = BenefConfigDAO.Instance.GetForControl(TipoBenef);
            int numLinhas = (int)Math.Floor((double)lstBenef.Count / 2);
            if ((float)lstBenef.Count % 2 > 0)
                numLinhas++;

            if (_bloquearBeneficiamentos && !String.IsNullOrEmpty(_mensagemBloqueioBenef))
                tblBenef.Caption = "<div style=\"padding: 8px; font-size: 120%; font-weight: bold\">" + _mensagemBloqueioBenef + "</div>";

            // Percorre o número de linhas
            for (int i = 0; i < numLinhas; i++)
            {
                // Cria a linha e as células e adiciona-os à tabela
                TableRow linha = new TableRow();
                TableCell cabecalho1 = new TableCell();
                TableCell cabecalho2 = new TableCell();
                TableCell controles1 = new TableCell();
                TableCell controles2 = new TableCell();
                linha.Cells.AddRange(new TableCell[] { cabecalho1, controles1, cabecalho2, controles2 });
                tblBenef.Rows.Add(linha);

                // Define os estilos das linhas (normal ou alternada)
                if (i % 2 == 0)
                {
                    if (!String.IsNullOrEmpty(_cssClassLinha))
                        linha.CssClass = _cssClassLinha;
                }
                else
                {
                    if (!String.IsNullOrEmpty(_cssClassLinhaAlternada))
                        linha.CssClass = _cssClassLinhaAlternada;
                }

                // Cria os controles do beneficiamento para as primeiras células
                CreateBenef(cabecalho1, controles1, lstBenef[i]);

                // Define o estilo dos controles da primeira coluna
                controles1.Style.Add("Padding-right", "8px");

                // Verifica se há um beneficiamento para as próximas colunas
                if ((i + numLinhas) >= lstBenef.Count)
                    break;

                // Cria os controles do beneficiamento para as últimas células
                CreateBenef(cabecalho2, controles2, lstBenef[i + numLinhas]);
            }
        }
    
        protected void tblBenef_Load(object sender, EventArgs e)
        {
            if (_beneficiamentos2 != null && IsPostBack)
            {
                // Cria a variável de retorno
                var retorno = new List<Glass.Global.Negocios.Entidades.IBeneficiamento>();

                // Percorre as linhas da tabela
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui no retorno os beneficiamentos da primeira célula
                    retorno.AddRange(Converter(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[1])));

                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;

                    // Inclui no retorno os beneficiamentos da segunda célula
                    retorno.AddRange(Converter(GetBeneficiamentosFromCell(tblBenef.Rows[i].Cells[3])));
                }

                (_beneficiamentos2 as IList).ApplyDiff<Glass.Global.Negocios.Entidades.IBeneficiamento>
                    (retorno, (x, y) => x.Equals(y));
            }
        }
    
        protected void ctrlBenef_PreRender(object sender, EventArgs e)
        {
            // Registra os controles que serão usados pelo JavaScript
            string formato = @"
                Altura: '{0}', 
                Largura: '{1}', 
                Espessura: '{2}', 
                PercComissao: '{3}', 
                Quantidade: '{4}', 
                TipoEntrega: '{5}', 
                TotalM2: '{6}', 
                ValorUnitario: '{7}', 
                ProdutoID: '{8}', 
                ClienteID: '{9}', 
                ItensVazios: {10}, 
                Beneficiamentos: {11}, 
                Revenda: '{12}', 
                Compra: {13}, 
                CarregarBenefPadrao: {14}, 
                BloquearBenef: {15}, 
                AlterarBenefAssoc: {16}, 
                NumeroBeneficiamentos: {17}, 
                Limpar: {18}, 
                CarregarBeneficiamentos: {19}, 
                Servicos: {20}, 
                PecaItemProjetoID: '{21}', 
                AplicacaoID: '{22}', 
                ProcessoID: '{23}', 
                Aplicacao: '{24}', 
                Processo: '{25}', 
                Custo: '{26}', 
                BeneficiamentosApenasVidros: {27}, 
                SomarTotalValorBenef: {28}, 
                CalcularBeneficiamentoPadrao: {29},
                QuantidadeAmbiente: '{30}'";
                
            // Formata os controles, colocando a função de cálculo quando os campos forem alterados
            FormatControl(_campoAltura);
            FormatControl(_campoEspessura);
            FormatControl(_campoLargura);
            FormatControl(_campoPercComissao);
            FormatControl(_campoQuantidade);
            FormatControl(_campoTipoEntrega);
            FormatControl(_campoTotalM2);
            FormatControl(_campoValorUnitario);
            FormatControl(_campoCusto);
            FormatControl(_campoIdProduto);
            FormatControl(_campoRevenda);
            FormatControl(_campoIdCliente);
            FormatControl(_campoIdPecaItemProjeto);
    
            // Define o grupo de validação da espessura do vidro
            ctvEspessura.ValidationGroup = vsuSumario.ValidationGroup;
    
            // Cria a linha do script com a variável do controle
            object[] dadosFormato = new object[] {
                GetControlID(_campoAltura),
                GetControlID(_campoLargura),
                GetControlID(_campoEspessura),
                GetControlID(_campoPercComissao),
                GetControlID(_campoQuantidade),
                GetControlID(_campoTipoEntrega),
                GetControlID(_campoTotalM2),
                GetControlID(_campoValorUnitario),
                GetControlID(_campoIdProduto),
                GetControlID(_campoIdCliente),
                GetItensVazios(),
                GetConfig(false),
                GetControlID(_campoRevenda),
                _isCompra.ToString().ToLower(),
                _carregarBenefPadrao.ToString().ToLower(),
                _bloquearBeneficiamentos.ToString().ToLower(),
                "true",
                "function() { return getNumeroBeneficiamentos('" + this.ClientID + "'); }",
                "function() { limparBenef('" + this.ClientID + "_', '" + (!String.IsNullOrEmpty(_callbackCalculoValorTotal) ? _callbackCalculoValorTotal : "") + "') }",
                "function(id, tipo) { carregarBeneficiamentos('" + this.ClientID + "', id, tipo, false); }",
                "function() { return getServicos('" + this.ClientID + "'); }",
                GetControlID(_campoIdPecaItemProjeto),
                GetControlID(_campoIdAplicacao),
                GetControlID(_campoIdProcesso),
                GetControlID(_campoAplicacao),
                GetControlID(_campoProcesso),
                GetControlID(_campoCusto),
                (!Geral.UsarBeneficiamentosTodosOsGrupos).ToString().ToLower(),
                _somarTotalValorBenef.ToString().ToLower(),
                _calcularBeneficiamentoPadrao.ToString().ToLower(),
                GetControlID(_campoQuantidadeAmbiente)
            };
    
            // Define o script da variável na tela
            string script = "var " + this.ClientID + " = { " + String.Format(formato, dadosFormato) + " };\n";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, script, true);
    
            // Define o script de inicialização do controle na tela
            script = GetFuncaoCalculo(null) + ";\niniciando = false;\n";
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_Load", script, true);

            if (_beneficiamentos2 != null)
            {
                var beneficiamentos = _beneficiamentos2
                    .Select(f => new GenericBenef
                    {
                        BisAlt = f.BisAlt,
                        BisLarg = f.BisLarg,
                        Custo = f.Custo,
                        EspBisote = f.EspBisote,
                        EspFuro = f.EspFuro,
                        IdBenefConfig = (uint)f.IdBenefConfig,
                        LapAlt = f.LapAlt,
                        LapLarg = f.LapLarg,
                        Padrao = f.Padrao,
                        Qtd = f.Qtd,
                        TipoProdutoBenef = f.TipoProdutoBenef,
                        Valor = f.Valor,
                        ValorAcrescimo = f.ValorAcrescimo,
                        ValorAcrescimoProd = f.ValorAcrescimoProd,
                        ValorComissao = f.ValorComissao,
                        ValorDesconto = f.ValorDesconto,
                        ValorDescontoProd = f.ValorDescontoProd,
                        ValorUnit = f.ValorUnit
                    }).ToList();

                // Percorre as linhas da coluna
                for (int i = 0; i < tblBenef.Rows.Count; i++)
                {
                    // Inclui na primeira célula os beneficiamentos
                    SetBeneficiamentosToCell(beneficiamentos, tblBenef.Rows[i].Cells[1]);

                    // Garante que haja a próxima célula
                    if (tblBenef.Rows[i].Cells[3].Controls.Count == 0)
                        break;

                    // Inclui na segunda célula os beneficiamentos
                    SetBeneficiamentosToCell(beneficiamentos, tblBenef.Rows[i].Cells[3]);
                }
            }
        }
    }
}
