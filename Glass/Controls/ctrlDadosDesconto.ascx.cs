using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Glass.Configuracoes;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlDadosDesconto : BaseUserControl
    {
        #region Campos privados
    
        private bool _isPedidoFastDelivery;
        private float _taxaFastDelivery;
        private Control _campoFastDelivery;
        private Control _campoTotalSemDesconto;
        private Control _campoTipoDesconto;
        private Control _campoDesconto;
    
        #endregion
    
        #region Propriedades
    
        public bool IsPedidoFastDelivery
        {
            get { return _isPedidoFastDelivery; }
            set { _isPedidoFastDelivery = value; }
        }
    
        public float TaxaFastDelivery
        {
            get { return _taxaFastDelivery; }
            set 
            {
                _taxaFastDelivery = value;
                lblTaxaFastDelivery.Text = Math.Max(value, PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery).ToString() + "%"; 
            }
        }
    
        public Control CampoFastDelivery
        {
            get { return _campoFastDelivery; }
            set { _campoFastDelivery = value; }
        }
    
        [Bindable(false)]
        public Control CampoTotalSemDesconto
        {
            get { return _campoTotalSemDesconto; }
            set { _campoTotalSemDesconto = value; }
        }
    
        [Bindable(false)]
        public Control CampoTipoDesconto
        {
            get { return _campoTipoDesconto; }
            set { _campoTipoDesconto = value; }
        }
    
        [Bindable(false)]
        public Control CampoDesconto
        {
            get { return _campoDesconto; }
            set { _campoDesconto = value; }
        }
    
        #endregion
    
        #region M�todos de suporte
    
        /// <summary>
        /// Retorna a fun��o que calcula os dados do controle.
        /// </summary>
        /// <returns>O c�digo de JavaScript da fun��o.</returns>
        private string GetFuncaoCalculo()
        {
            return "calculaDadosDesconto('" + this.ClientID + "')";
        }
    
        /// <summary>
        /// Formata um controle da p�gina.
        /// </summary>
        /// <param name="campo">O controle da p�gina.</param>
        private void FormatControl(Control campo)
        {
            // Garante que o campo seja v�lido
            if (campo == null || !(campo is WebControl))
                return;
    
            // String com o atributo que ser� alterado
            string atributo;
            if (campo is DropDownList)
                atributo = "OnChange";
            else if (campo is CheckBox)
                atributo = "OnClick";
            else
                atributo = "OnBlur";
    
            // String com a fun��o que ser� executada
            string funcao = "";
    
            // Verifica se o controle j� possui uma fun��o atribu�da ao evento OnBlur
            if (!String.IsNullOrEmpty(((WebControl)campo).Attributes[atributo]))
            {
                // Recupera a fun��o do controle
                funcao = ((WebControl)campo).Attributes[atributo];
    
                // Verifica se a fun��o desejada j� est� no controle
                if (funcao.IndexOf(GetFuncaoCalculo()) > -1)
                    return;
    
                // Coloca a fun��o de c�lculo junto � fun��o original
                if (funcao.IndexOf("return") > -1)
                    funcao = funcao.Replace("return", GetFuncaoCalculo() + "; return");
                else
                    funcao += "; " + GetFuncaoCalculo();
            }
    
            // Indica que apenas essa fun��o ser� executada
            else
                funcao = GetFuncaoCalculo();
    
            // Atribui a fun��o ao controle
            if (((WebControl)campo).Attributes[atributo] == null || !((WebControl)campo).Attributes[atributo].Contains(funcao))
                ((WebControl)campo).Attributes[atributo] = funcao;
        }
    
        /// <summary>
        /// Retorna o ClientID de um controle da p�gina.
        /// </summary>
        /// <param name="campo">O controle da p�gina.</param>
        /// <returns>Uma string com o ClientID do controle.</returns>
        private string GetControlID(Control campo)
        {
            // Garante que o campo seja v�lido
            if (campo == null)
                return "";
    
            // Retorna o identificador do campo na p�gina cliente
            return campo.ClientID;
        }
    
        #endregion
    
        protected void Page_Load(object sender, EventArgs e)
        {
            // Registra os scripts
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("ctrlDadosDesconto"))
                Page.ClientScript.RegisterClientScriptInclude("ctrlDadosDesconto", ResolveClientUrl("~/Scripts/ctrlDadosDesconto.js"));
    
            this.PreRender += new EventHandler(ctrlDadosDesconto_PreRender);
        }
    
        protected void ctrlDadosDesconto_PreRender(object sender, EventArgs e)
        {
            string formato = "" +
                "CampoDesconto: '{0}', " +
                "CampoFastDelivery: '{1}', " +
                "CampoTipoDesconto: '{2}', " +
                "CampoTotalSemDesconto: '{3}', " +
                "TaxaFastDelivery: {4}, " +
                "IsPedidoFastDelivery: {5}";
    
            FormatControl(_campoDesconto);
            FormatControl(_campoFastDelivery);
            FormatControl(_campoTipoDesconto);
            FormatControl(_campoTotalSemDesconto);
    
            object[] dadosFormato = new object[6];
            dadosFormato[0] = GetControlID(_campoDesconto);
            dadosFormato[1] = GetControlID(_campoFastDelivery);
            dadosFormato[2] = GetControlID(_campoTipoDesconto);
            dadosFormato[3] = GetControlID(_campoTotalSemDesconto);
            dadosFormato[4] = TaxaFastDelivery.ToString().Replace(",", ".");
            dadosFormato[5] = _isPedidoFastDelivery.ToString().ToLower();
    
            Page.ClientScript.RegisterClientScriptBlock(GetType(), this.ClientID, "var " + this.ClientID + " = { " + 
                String.Format(formato, dadosFormato) + " };\n", true);
    
            Page.ClientScript.RegisterStartupScript(GetType(), this.ClientID + "_start", GetFuncaoCalculo() + ";\n", true);
        }
    }
}
