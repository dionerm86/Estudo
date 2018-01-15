using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Glass.UI.Web.Controls
{
    public partial class ctrlData : BaseUserControl
    {
        public enum ReadOnlyEnum
        {
            ReadOnly,
            ReadErase,
            ReadWrite
        }
    
        #region Campos privados
    
        private ReadOnlyEnum _readOnly = ReadOnlyEnum.ReadErase;
        private string _callbackSelecionaData = String.Empty;

        #endregion

        #region Propriedades

        public bool ExibirHoras
        {
            get { return txtHora.Visible; }
            set
            {
                txtHora.Visible = value;
                ctvHora.Enabled = value;
            }
        }
    
        public string CallbackSelecionaData
        {
            get { return _callbackSelecionaData; }
            set { _callbackSelecionaData = value; }
        }

        public ReadOnlyEnum ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        public DateTime Data
        {
            get { return DateTime.Parse(DataString); }
            set { DataString = value.ToString("dd/MM/yyyy" + (ExibirHoras ? " HH:mm" : "")); }
        }
    
        public DateTime? DataNullable
        {
            get { return Conversoes.ConverteData(DataString); }
            set { DataString = Conversoes.ConverteData(value, ExibirHoras); }
        }
    
        public string DataString
        {
            get { return (txtData.Text + (ExibirHoras ? " " + txtHora.Text : "")).Trim(); }
            set
            {
                if (value == null)
                {
                    txtData.Text = "";
                    return;
                }
    
                string[] data = value.Split(' ');
                txtData.Text = data[0];
                txtHora.Text = data.Length > 1 ? data[1] : "";
            }
        }
    
        public bool ValidateEmptyText
        {
            get { return ctvData.ValidateEmptyText; }
            set
            {
                ctvData.ValidateEmptyText = value;
                ctvHora.ValidateEmptyText = value;
            }
        }
    
        public string ErrorMessage
        {
            get { return ctvData.ErrorMessage; }
            set
            {
                ctvData.ErrorMessage = value;
                ctvHora.ErrorMessage = value;
            }
        }
    
        public string ValidationGroup
        {
            get { return ctvData.ValidationGroup; }
            set
            {
                ctvData.ValidationGroup = value;
                ctvHora.ValidationGroup = value;
            }
        }
    
        public override bool EnableViewState
        {
            get { return base.EnableViewState; }
            set
            {
                base.EnableViewState = value;
                txtData.EnableViewState = value;
                txtHora.EnableViewState = value;
                imgData.EnableViewState = value;
                ctvData.EnableViewState = value;
                ctvHora.EnableViewState = value;
            }
        }
    
        public bool Enabled
        {
            get { return txtData.Enabled; }
            set
            {
                txtData.Enabled = value;
                txtHora.Enabled = value;
                imgData.Enabled = value;
            }
        }
    
        public CustomValidator[] Validadores
        {
            get { return new[] { ctvData, ctvHora }; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            txtData.ValidationGroup = ValidationGroup;

            string validationGroup = !String.IsNullOrEmpty(ValidationGroup) ? "'" + ValidationGroup + "'" : "null";
            imgData.OnClientClick = "return SelecionaDataControle('" + this.ClientID + "_txtData" + "', this, " +
                validationGroup + ", '" + ctvData.ClientID + "')";

            if (ExibirHoras)
                txtData.Attributes.Add("OnKeyUp", "if (this.value.length == 10) document.getElementById('" + txtHora.ClientID + "').focus()");

            /* Chamado 43666. */
            if (_readOnly == ReadOnlyEnum.ReadOnly)
            {
                // Impede que o valor do campo data seja alterado via Ctrl + v.
                txtData.Attributes.Add("OnPaste", "return false;");
                // Impede que o valor do campo data seja recortado.
                txtData.Attributes.Add("OnCut", "return false;");
            }

            string funcaoReadOnlyData = _readOnly == ReadOnlyEnum.ReadOnly ? "return false" :
                _readOnly == ReadOnlyEnum.ReadErase ? "return soSistema(event, true)" :
                "return mascara_data(event, this)";
    
            string funcaoReadOnlyHora = _readOnly == ReadOnlyEnum.ReadOnly ? "return false" :
                _readOnly == ReadOnlyEnum.ReadErase ? "return soSistema(event, true)" :
                "return mascara_hora(event, this)";
    
            txtData.Attributes.Add("OnKeyDown", funcaoReadOnlyData);
            txtHora.Attributes.Add("OnKeyDown", funcaoReadOnlyHora);
    
            string onChange = CallbackSelecionaData + "ValidaDataControle('" + txtData.ClientID + "', " + validationGroup + ", '" + ctvData.ClientID + "')";
            if (this.Attributes["onchange"] != null)
                onChange += "; " + this.Attributes["onchange"];
            
            txtData.Attributes.Add("OnDateChange", onChange);
    
            if (!Page.ClientScript.IsClientScriptBlockRegistered(GetType(), "ctrlData_Script"))
            {
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlData_CSS_Javascript",
                    "<link href='" + ResolveUrl("~/Style/dhtmlgoodies_calendar.css?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) + @"' type='text/css' rel='Stylesheet'>
                     <script type='text/javascript' src='" + ResolveUrl("~/Scripts/dhtmlgoodies_calendar.js?v=" + Glass.Configuracoes.Geral.ObtemVersao(true)) + "'></script>", false);
    
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ctrlData_Script", @"
                    function ValidaDataControle(campoData, grupo, valData)
                    {
                        valData = document.getElementById(valData);
                        
                        if (valData != null)
                        {
                            ValidatorValidate(valData, grupo, null);
                            
                            if (!valData.isvalid)
                                document.getElementById(campoData).value = '';
                        }
                    }
                    
                    function SelecionaDataControle(campoData, botao, grupo, valData)
                    {
                        ValidaDataControle(campoData, grupo, valData);
                        SelecionaData(campoData, botao);
                        return false;
                    }
                    function validaData(val, args)
                    {
                        args.IsValid = isDataValida(args.Value);
                    }
                    function validaHora(val, args)
                    {
                        args.IsValid = isHoraValida(args.Value);
                    }", true);
            }
        }
    }
}
