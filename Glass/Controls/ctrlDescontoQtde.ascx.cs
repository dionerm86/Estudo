using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlDescontoQtde : BaseUserControl
    {
        #region Campos privados
    
        private Control _campoIdProd;
        private Control _campoQtde;
        private Control _campoIdCliente;
        private Control _campoTipoEntrega;
        private Control _campoRevenda;
        private Control _campoReposicao;
        private Control _campoValorUnit;
        private Control _campoTotal;
    
        private string _callback;
        private string _callbackValorUnit;
        private bool _executarOnChangeValorUnitario = false;
        private bool _forcarEsconderControle = false;
        private bool _bloquearAlteracaoDesconto = false;

        #endregion

        #region Propriedades

        public Control CampoProdutoID
        {
            get { return _campoIdProd; }
            set { _campoIdProd = value; }
        }
    
        public Control CampoQtde
        {
            get { return _campoQtde; }
            set { _campoQtde = value; }
        }
    
        public Control CampoClienteID
        {
            get { return _campoIdCliente; }
            set { _campoIdCliente = value; }
        }
    
        public Control CampoTipoEntrega
        {
            get { return _campoTipoEntrega; }
            set { _campoTipoEntrega = value; }
        }
    
        public Control CampoRevenda
        {
            get { return _campoRevenda; }
            set { _campoRevenda = value; }
        }
    
        public Control CampoReposicao
        {
            get { return _campoReposicao; }
            set { _campoReposicao = value; }
        }
    
        public Control CampoValorUnit
        {
            get { return _campoValorUnit; }
            set { _campoValorUnit = value; }
        }
    
        public Control CampoTotal
        {
            get { return _campoTotal; }
            set { _campoTotal = value; }
        }
    
        public float PercDescontoQtde
        {
            get 
            { 
                string percDescontoMax = _campoIdProd == null || _campoQtde == null ? "0" :
                    GetPercDescontoQtde(GetControlValue(_campoIdProd), GetControlValue(_campoQtde)).Split(';')[1];
    
                if (percDescontoMax != null && percDescontoMax.ToLower().Contains("erro"))
                    return 0;
    
                return Glass.Conversoes.StrParaFloat(percDescontoMax) > 0 ? Glass.Conversoes.StrParaFloat(txtPercDescQtde.Text) : 0;
            }
            set { txtPercDescQtde.Text = value.ToString(); }
        }
    
        public decimal ValorDescontoQtde
        {
            get { return Glass.Conversoes.StrParaDecimal(hdfValorDescontoQtde.Value); }
            set { hdfValorDescontoQtde.Value = value.ToString(); }
        }
    
        public string Callback
        {
            get { return !String.IsNullOrEmpty(_callback) ? "'" + _callback + "'" : "''"; }
            set { _callback = value; }
        }
    
        public string CallbackValorUnit
        {
            get { return !String.IsNullOrEmpty(_callbackValorUnit) ? "'" + _callbackValorUnit + "'" : "''"; }
            set { _callbackValorUnit = value; }
        }
    
        public string ValidationGroup
        {
            get { return ctvPercDesconto.ValidationGroup; }
            set { ctvPercDesconto.ValidationGroup = value; }
        }
    
        public bool ExecutarOnChangeValorUnitario
        {
            get { return _executarOnChangeValorUnitario; }
            set { _executarOnChangeValorUnitario = value; }
        }
    
        public bool ForcarEsconderControle
        {
            get { return _forcarEsconderControle; }
            set { _forcarEsconderControle = value; }
        }

        public bool BloquearAlteracaoDesconto
        {
            get { return _bloquearAlteracaoDesconto; }
            set { _bloquearAlteracaoDesconto = value; }
        }
    
        #endregion
    
        #region Métodos de suporte
    
        /// <summary>
        /// Retorna a função de cálculo do controle.
        /// </summary>
        /// <returns></returns>
        private string GetFuncaoCalculo()
        {
            return "descQtde_getDescontoQtde('" + this.ClientID + "', " + PedidoConfig.Desconto.DescontoPorProduto.ToString().ToLower() + ", false, " + 
                ForcarEsconderControle.ToString().ToLower() + ", " + Callback + ", " + CallbackValorUnit + ")";
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
        /// Retorna o valor do campo.
        /// </summary>
        /// <param name="campo"></param>
        /// <returns></returns>
        private string GetControlValue(Control campo)
        {
            if (campo == null)
                return "";
    
            if (campo is TextBox)
                return ((TextBox)campo).Text;
            else if (campo is HiddenField)
                return ((HiddenField)campo).Value;
            else if (campo is DropDownList)
                return ((DropDownList)campo).SelectedValue;
            else if (campo is Label)
                return ((Label)campo).Text;
    
            return "";
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public static string GetValorTabela(string idProdStr, string tipoEntregaStr, string idClienteStr, string revendaStr, 
            string reposicaoStr, string percDescontoQtdeStr)
        {
            int idProd = !String.IsNullOrEmpty(idProdStr) ? Glass.Conversoes.StrParaInt(idProdStr) : 0;
            int? tipoEntrega = !String.IsNullOrEmpty(tipoEntregaStr) ? (int?)Glass.Conversoes.StrParaInt(tipoEntregaStr) : null;
            uint? idCliente = !String.IsNullOrEmpty(idClienteStr) ? (uint?)Glass.Conversoes.StrParaUint(idClienteStr) : null;
            bool revenda = !String.IsNullOrEmpty(revendaStr) ? bool.Parse(revendaStr) : false;
            bool reposicao = !String.IsNullOrEmpty(reposicaoStr) ? bool.Parse(reposicaoStr) : false;
            float percDescontoQtde = Glass.Conversoes.StrParaFloat(percDescontoQtdeStr);
    
            return ProdutoDAO.Instance.GetValorTabela(idProd, tipoEntrega, idCliente, revenda, reposicao, percDescontoQtde, null, null, null).ToString("0.00");
        }
    
        [Ajax.AjaxMethod]
        public static string GetDescontoTabela(string idProdStr, string idClienteStr)
        {
            int idProd = !String.IsNullOrEmpty(idProdStr) ? Glass.Conversoes.StrParaInt(idProdStr) : 0;
            uint? idCliente = !String.IsNullOrEmpty(idClienteStr) ? (uint?)Glass.Conversoes.StrParaUint(idClienteStr) : null;
    
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(idProd);
    
            Glass.Data.Model.DescontoAcrescimoCliente desc = DescontoAcrescimoClienteDAO.Instance.GetDescontoAcrescimo(
                idCliente > 0 ? idCliente.Value : 0, idGrupoProd, idSubgrupoProd, idProd, null, null);
    
            return desc.Desconto.ToString();
        }
    
        [Ajax.AjaxMethod]
        public static string GetPercDescontoQtde(string idProdStr, string qtdeStr)
        {
            try
            {
                uint idProd = !String.IsNullOrEmpty(idProdStr) ? Glass.Conversoes.StrParaUint(idProdStr) : 0;
                int qtde = !String.IsNullOrEmpty(qtdeStr) ? Glass.Conversoes.StrParaInt(qtdeStr) : 0;
    
                float percDesconto = 0;
                if (PedidoConfig.Desconto.DescontoPorProduto)
                    percDesconto = DescontoQtdeDAO.Instance.GetPercDescontoByProd(idProd, qtde);
    
                return "Ok;" + percDesconto.ToString();
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao buscar desconto por quantidade de produtos.", ex);
            }
        }
    
        [Ajax.AjaxMethod]
        public static string ImpedirDescontoSomativo()
        {
            return PedidoConfig.Desconto.ImpedirDescontoSomativo.ToString().ToLower();
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("ctrlDescontoQtde"))
            {
                Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlDescontoQtde));
                Page.ClientScript.RegisterClientScriptInclude("ctrlDescontoQtde", this.ResolveClientUrl("~/Scripts/ctrlDescontoQtde.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)));
            }
    
            Page.PreRender += new EventHandler(Page_PreRender);
        }
    
        private void Page_PreRender(object sender, EventArgs e)
        {
            // Coloca o callback no campo de percentual de desconto
            txtPercDescQtde.Attributes.Add("OnChange", GetFuncaoCalculo());

            if (BloquearAlteracaoDesconto)
                txtPercDescQtde.Enabled = false;

            // Variável de formato
            string formato = "" +
                "ProdutoID: '{0}', " +
                "Quantidade: '{1}', " +
                "ClienteID: '{2}', " +
                "TipoEntrega: '{3}', " +
                "Revenda: '{4}', " +
                "ValorUnitario: '{5}', " +
                "PercDescontoAtualInterno: {6}, " +
                "PercDescontoAtual: {7}, " +
                "PercDesconto: {8}, " +
                "Verificar: {9}, " +
                "RetornarDescontoAtual: {10}, " +
                "ValorDescontoAtual: {11}, " +
                "Total: '{12}', " +
                "ExecutarOnChangeValorUnitario: {13}, " +
                "Reposicao: '{14}'";
    
            FormatControl(_campoIdProd);
            FormatControl(_campoQtde);
            FormatControl(_campoIdCliente);
            FormatControl(_campoTipoEntrega);
            FormatControl(_campoRevenda);
            FormatControl(_campoReposicao);
    
            object[] dadosFormato = new object[15];
            dadosFormato[0] = GetControlID(_campoIdProd);
            dadosFormato[1] = GetControlID(_campoQtde);
            dadosFormato[2] = GetControlID(_campoIdCliente);
            dadosFormato[3] = GetControlID(_campoTipoEntrega);
            dadosFormato[4] = GetControlID(_campoRevenda);
            dadosFormato[5] = GetControlID(_campoValorUnit);
            dadosFormato[6] = PercDescontoQtde.ToString().Replace(",", ".");
            dadosFormato[7] = "function() { return descQtde_getPercDescontoQtdeAtual('" + this.ClientID + "') }";
            dadosFormato[8] = "function() { return descQtde_getPercDescontoQtde('" + this.ClientID + "') }";
            dadosFormato[9] = "function() { return " + GetFuncaoCalculo() + " }";
            dadosFormato[10] = "true";
            dadosFormato[11] = ValorDescontoQtde.ToString().Replace(",", ".");
            dadosFormato[12] = GetControlID(_campoTotal);
            dadosFormato[13] = _executarOnChangeValorUnitario.ToString().ToLower();
            dadosFormato[14] = GetControlID(_campoReposicao);
    
            string script = "var " + this.ClientID + " = { " + String.Format(formato, dadosFormato) + " };\n";
            //script += "controlesDescontoQtde.push('" + this.ClientID + "');\n";
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, script, true);
    
            Page.ClientScript.RegisterStartupScript(GetType(), "init_" + this.ClientID, GetFuncaoCalculo() + ";\n", true);
            //Page.ClientScript.RegisterOnSubmitStatement(GetType(), "submitDescontoQtde", "atualizaTotalDescontoQtde();\n");
        }
    }
}
