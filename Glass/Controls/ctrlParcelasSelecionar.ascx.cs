using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlParcelasSelecionar : BaseUserControl
    {
        #region Campos privados
    
        private string _callbackSelecaoParcelas = null;
        private Controls.ctrlParcelas _controleParcelas;
        private bool _exibirParcConfiguravel = false;
        private bool _sempreExibirDatasParcelas = false;
        private bool _recalcularParcelasApenasTrocaValor = false;
        private string _parentId = "";
    
        private Control _campoIdCliente;
        private Control _campoIdFornecedor;
        private Control _campoIdsPedidos;
    
        #endregion
    
        #region Propriedades
    
        /// <summary>
        /// O ID do controle pai.
        /// </summary>
        public string ParentID
        {
            get { return _parentId; }
            set { _parentId = value; }
        }
    
        public uint? ParcelaPadrao
        {
            get { return ParcelaPadraoInt == null || ParcelaPadraoInt == -1 ? null : (uint?)ParcelaPadraoInt; }
            set { ParcelaPadraoInt = (int?)value; }
        }
    
        public int? ParcelaPadraoInt
        {
            get { return !String.IsNullOrEmpty(hdfIdParcela.Value) ? (int?)Glass.Conversoes.StrParaInt(hdfIdParcela.Value) : null; }
            set { hdfIdParcela.Value = value != null ? value.ToString() : ExibirParcConfiguravel ? "-1" : ""; }
        }
    
        public string CallbackSelecaoParcelas
        {
            get { return !String.IsNullOrEmpty(_callbackSelecaoParcelas) ? "'" + _callbackSelecaoParcelas + "'" : "''"; }
            set { _callbackSelecaoParcelas = value; }
        }
    
        public Controls.ctrlParcelas ControleParcelas
        {
            get { return _controleParcelas; }
            set { _controleParcelas = value; }
        }
    
        public int NumeroParcelas
        {
            get { return Glass.Conversoes.StrParaInt(hdfNumParcelas.Value); }
            set { hdfNumParcelas.Value = value.ToString(); }
        }
    
        public int NumeroParcelasMaximo
        {
            get { return _controleParcelas != null ? _controleParcelas.NumParcelas : 0; }
            set { if (_controleParcelas != null) _controleParcelas.NumParcelas = value; }
        }
    
        public bool ExibirParcConfiguravel
        {
            get { return _exibirParcConfiguravel; }
            set { _exibirParcConfiguravel = value; }
        }
    
        public bool SempreExibirDatasParcelas
        {
            get { return _sempreExibirDatasParcelas; }
            set { _sempreExibirDatasParcelas = value; }
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
    
        public ParcelasDAO.TipoConsulta TipoConsulta
        {
            get { return (ParcelasDAO.TipoConsulta)Glass.Conversoes.StrParaInt(hdfTipoConsulta.Value); }
            set { hdfTipoConsulta.Value = ((int)value).ToString(); }
        }

        public int? IdParcela
        {
            get
            {
                return drpParcelas.SelectedValue.StrParaIntNullable();
            }
        }
    
        #endregion
    
        #region Propriedades dos controles
    
        public Control CampoClienteID
        {
            get { return _campoIdCliente; }
            set { _campoIdCliente = value; }
        }
    
        public Control CampoFornecedorID
        {
            get { return _campoIdFornecedor; }
            set { _campoIdFornecedor = value; }
        }
    
        public Control CampoPedidosIDs
        {
            get { return _campoIdsPedidos; }
            set { _campoIdsPedidos = value; }
        }
    
        #endregion
    
        #region Métodos privados
    
        /// <summary>
        /// Formata um controle da página.
        /// </summary>
        /// <param name="campo">O controle da página.</param>
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
        /// Formata o controle de parcelas vinculado.
        /// </summary>
        /// <param name="controleParcelas"></param>
        private void FormatControleParcelas()
        {
            // Garante que o campo seja válido
            if (_controleParcelas == null)
                return;
    
            // Define o campo com o número de parcelas visíveis
            _controleParcelas.CampoParcelasVisiveis = hdfNumParcelas;
            _controleParcelas.CampoTextoParcelas = hdfDiasParcelas;
    
            // String com a função que será executada
            string callback = this.ClientID + ".Calcular";
            string funcao = _controleParcelas.GetFuncaoCalculo();
    
            // Define a função de cálculo como callback do controle de parcelas
            if (_controleParcelas.CallbackTotal == "''")
                _controleParcelas.CallbackTotal = callback;
            else if (!_controleParcelas.CallbackTotal.Contains(callback))
                _controleParcelas.CallbackTotal = callback + "(); " + _controleParcelas.CallbackTotal.Replace("'", "");
    
            // Campos que serão formatados
            drpNumParcCustom.Items.Clear();
            for (int i = 0; i < _controleParcelas.NumParcelas; i++)
                drpNumParcCustom.Items.Add(new ListItem((i + 1).ToString()));
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
        /// Retorna os dados das parcelas.
        /// </summary>
        /// <returns></returns>
        private string GetDadosParcelas(IEnumerable<Parcelas> parcelas, uint? parcelaPadrao)
        {
            string retorno = "";
    
            foreach (Parcelas p in parcelas)
            {
                retorno += ", { ";
                retorno += "ID: " + p.IdParcela;
                retorno += ", NumParcelas: " + p.NumParcelas;
                retorno += ", Dias: '" + p.Dias + "'";
                retorno += ", Padrao: " + (parcelaPadrao > 0 && p.IdParcela == parcelaPadrao).ToString().ToLower();
                retorno += " }";
            }
    
            return "new Array(" + (retorno.Length > 0 ? retorno.Substring(2) : "") + ")";
        }
    
        /// <summary>
        /// Retorna a função principal do controle.
        /// </summary>
        /// <returns></returns>
        private string GetFuncaoCalculo()
        {
            return "alteraParcela('" + this.ClientID + "', " + CallbackSelecaoParcelas + ")";
        }
    
        #endregion
    
        #region Métodos de suporte
    
        /// <summary>
        /// O controle de parcelas exibe os dias das parcelas?
        /// </summary>
        /// <returns></returns>
        public bool ExibirDias()
        {
            return _sempreExibirDatasParcelas || PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido;
        }
    
        #endregion
    
        #region Métodos Ajax
    
        [Ajax.AjaxMethod]
        public string GetParcelasCliFornec(string idClienteStr, string idFornecStr, string idsPedidos, 
            string exibirParcConfiguravel, string exibirDatas, string tipoConsulta)
        {
            try
            {
                uint idCliente = Glass.Conversoes.StrParaUint(idClienteStr);
                uint idFornec = Glass.Conversoes.StrParaUint(idFornecStr);
    
                uint? parcelaPadrao = idCliente > 0 ? ClienteDAO.Instance.ObtemTipoPagto(idCliente) :
                    idFornec > 0 ? FornecedorDAO.Instance.ObtemTipoPagto(null, idFornec) : null;
    
                var retorno = "";
                var msgErro = "";
                var parcelas = ParcelasDAO.Instance.GetForControleSelecionar(idCliente, idFornec, idsPedidos, 
                    exibirDatas == "true", (ParcelasDAO.TipoConsulta)Glass.Conversoes.StrParaInt(tipoConsulta), out msgErro);
    
                // A parcela à vista não possui número de parcelas e como o método GetForControleSelecionar busca
                // somente parcelas com número de parcelas estava ocorrendo um erro ao tentar recuperar o IdParcela
                // da parcela da posição "0", caso isso ocorra não será retornada mensagem de erro.
                if (parcelas.Count == 0)
                    return "Ok##";
    
                if (!String.IsNullOrEmpty(idsPedidos) && Liberacao.DadosLiberacao.UsarMenorPrazoLiberarPedido && parcelas.Count > 0)
                    parcelaPadrao = (uint)parcelas[0].IdParcela;
    
                foreach (Parcelas p in parcelas)
                {
                    retorno += String.Format("<option value='{0}'{2}>{1}</option>", p.IdParcela, p.Descricao,
                        parcelaPadrao > 0 && p.IdParcela == parcelaPadrao ? " selected='selected'" : "");
                }
    
                if (exibirParcConfiguravel == "true")
                    retorno += "<option value='-1'>Configurável</option>";
    
                return "Ok#" + retorno + "#" + GetDadosParcelas(parcelas, parcelaPadrao) + "#" + msgErro;
            }
            catch (Exception ex)
            {
                return "Erro#" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao carregar parcelas.", ex);
            }
        }
    
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Controls.ctrlParcelasSelecionar));
    
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(GetType(), this.ClientID))
                Page.ClientScript.RegisterClientScriptInclude(GetType(), this.ClientID, ResolveClientUrl("~/Scripts/ctrlParcelasSelecionar.js"));
    
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_init", GetFuncaoCalculo() + ";\n" +
                this.ClientID + ".AtualizarParcelas();\n", true);
    
            Page.PreRender += new EventHandler(Page_PreRender);
        }
    
        protected void Page_PreRender(object sender, EventArgs e)
        {
            string formato = "" +
                "ControleParcelas: '{0}', " +
                "Parcelas: {1}, " +
                "Calcular: {2}, " +
                "DesabilitarControleParcelas: {3}, " +
                "AtualizarParcelas: {4}, " +
                "CampoClienteID: '{5}', " +
                "CampoFornecedorID: '{6}', " +
                "SempreExibirDatasParcelas: {7}, " +
                "ExibirParcelasConfiguraveis: {8}, " +
                "CampoPedidosIDs: '{9}', " +
                "ParentID: '{10}', " +
                "UsarMenorPrazoLiberarPedido: {11}";
    
            FormatControl(drpParcelas, (ExibirDias() ? "document.getElementById('" + hdfIdParcela.ClientID + "').value = this.value; " : "") + GetFuncaoCalculo());
    
            if (RecalcularParcelasApenasTrocaValor)
                drpParcelas.Attributes.Add("onclick", "calcParcelasLoad=true;");
    
            FormatControl(drpNumParcCustom, "alteraNumParcCustom('" + this.ClientID + "', " + CallbackSelecaoParcelas + ")");
            FormatControl(_campoIdCliente, this.ClientID + ".AtualizarParcelas()");
            FormatControl(_campoIdFornecedor, this.ClientID + ".AtualizarParcelas()");
            FormatControl(_campoIdsPedidos, this.ClientID + ".AtualizarParcelas()");
    
            FormatControleParcelas();
    
            bool exibirParcConfiguravel = _exibirParcConfiguravel && UserInfo.GetUserInfo.TipoUsuario == 
                (int)Data.Helper.Utils.TipoFuncionario.Administrador && _controleParcelas != null;
    
            object[] dadosFormato = new object[12];
            dadosFormato[0] = GetControlID(_controleParcelas);
            dadosFormato[1] = "new Array()";
            dadosFormato[2] = "function() { " + GetFuncaoCalculo() + " }";
            dadosFormato[3] = (_controleParcelas != null ? _controleParcelas.ReadOnly : false).ToString().ToLower();
            dadosFormato[4] = "function() { atualizaParcCliFornec('" + this.ClientID + "'); }";
            dadosFormato[5] = GetControlID(_campoIdCliente);
            dadosFormato[6] = GetControlID(_campoIdFornecedor);
            dadosFormato[7] = _sempreExibirDatasParcelas.ToString().ToLower();
            dadosFormato[8] = exibirParcConfiguravel.ToString().ToLower();
            dadosFormato[9] = GetControlID(_campoIdsPedidos);
            dadosFormato[10] = _parentId;
            dadosFormato[11] = Liberacao.DadosLiberacao.UsarMenorPrazoLiberarPedido.ToString().ToLower();
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID + "_var", "var " + this.ClientID + " = { " +
                String.Format(formato, dadosFormato) + " };\n", true);
    
            if (!ExibirDias())
            {
                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_padrao", "try { document.getElementById('" + drpParcelas.ClientID +
                    "').value = " + NumeroParcelas + "; } catch (err) { }\n", true);
            }
            else if (ParcelaPadraoInt > 0 || (ParcelaPadraoInt == -1 && _exibirParcConfiguravel))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_padrao", "try { var controle = document.getElementById('" + drpParcelas.ClientID +
                    "'); controle.value = " + ParcelaPadraoInt + "; controle.onchange(); } catch (err) { }\n", true);
    
                if (ParcelaPadraoInt == -1)
                    Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_numParcelas", "try { var controle = document.getElementById('" + drpNumParcCustom.ClientID +
                        "'); controle.value = " + NumeroParcelas + "; controle.onchange(); } catch (err) { }\n", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_padrao", "try { var controle = document.getElementById('" + drpParcelas.ClientID +
                    "'); controle.value = document.getElementById('" + hdfIdParcela.ClientID + "').value; controle.onchange(); } catch (err) { }\n", true);
            }
        }
    
        protected void drpParcelas_DataBound(object sender, EventArgs e)
        {
            if (_exibirParcConfiguravel)
                drpParcelas.Items.Add(new ListItem("Configurável", "-1"));
        }
    }
}
