﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace Glass.Data.wsPMSNFeConsultaCadastro {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="cadConsultaCadastroSoap12Binding", Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4")]
    public partial class CadConsultaCadastro4 : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback consultaCadastroOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public CadConsultaCadastro4() {
            this.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            this.Url = global::Glass.Data.Properties.Settings.Default.Glass_Data_wsPMSConsultaCadastro_CadConsultaCadastro2;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event consultaCadastroCompletedEventHandler consultaCadastroCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4/consultaCadastro", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4")]
        public consultaCadastroResult1 consultaCadastro([System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4")] consultaCadastroResult nfeDadosMsg) {
            object[] results = this.Invoke("consultaCadastro", new object[] {
                        nfeDadosMsg});
            return ((consultaCadastroResult1)(results[0]));
        }
        
        /// <remarks/>
        public void consultaCadastroAsync(consultaCadastroResult nfeDadosMsg) {
            this.consultaCadastroAsync(nfeDadosMsg, null);
        }
        
        /// <remarks/>
        public void consultaCadastroAsync(consultaCadastroResult nfeDadosMsg, object userState) {
            if ((this.consultaCadastroOperationCompleted == null)) {
                this.consultaCadastroOperationCompleted = new System.Threading.SendOrPostCallback(this.OnconsultaCadastroOperationCompleted);
            }
            this.InvokeAsync("consultaCadastro", new object[] {
                        nfeDadosMsg}, this.consultaCadastroOperationCompleted, userState);
        }
        
        private void OnconsultaCadastroOperationCompleted(object arg) {
            if ((this.consultaCadastroCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.consultaCadastroCompleted(this, new consultaCadastroCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2556.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4")]
    public partial class consultaCadastroResult {
        
        private System.Xml.XmlNode[] anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2556.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/CadConsultaCadastro4")]
    public partial class consultaCadastroResult1 {
        
        private System.Xml.XmlNode[] anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlNode[] Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void consultaCadastroCompletedEventHandler(object sender, consultaCadastroCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class consultaCadastroCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal consultaCadastroCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public consultaCadastroResult1 Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((consultaCadastroResult1)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591