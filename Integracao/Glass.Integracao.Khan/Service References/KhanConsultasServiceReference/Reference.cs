﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Glass.Integracao.Khan.KhanConsultasServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PedidoStatus", Namespace="http://schemas.datacontract.org/2004/07/KnIntegracao.Services")]
    [System.SerializableAttribute()]
    internal partial class PedidoStatus : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime datpedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int numpedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long numped_intField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int seqpedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int seqped_intField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string status_integracaoField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal System.DateTime datped {
            get {
                return this.datpedField;
            }
            set {
                if ((this.datpedField.Equals(value) != true)) {
                    this.datpedField = value;
                    this.RaisePropertyChanged("datped");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal int numped {
            get {
                return this.numpedField;
            }
            set {
                if ((this.numpedField.Equals(value) != true)) {
                    this.numpedField = value;
                    this.RaisePropertyChanged("numped");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal long numped_int {
            get {
                return this.numped_intField;
            }
            set {
                if ((this.numped_intField.Equals(value) != true)) {
                    this.numped_intField = value;
                    this.RaisePropertyChanged("numped_int");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal int seqped {
            get {
                return this.seqpedField;
            }
            set {
                if ((this.seqpedField.Equals(value) != true)) {
                    this.seqpedField = value;
                    this.RaisePropertyChanged("seqped");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal int seqped_int {
            get {
                return this.seqped_intField;
            }
            set {
                if ((this.seqped_intField.Equals(value) != true)) {
                    this.seqped_intField = value;
                    this.RaisePropertyChanged("seqped_int");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string status_integracao {
            get {
                return this.status_integracaoField;
            }
            set {
                if ((object.ReferenceEquals(this.status_integracaoField, value) != true)) {
                    this.status_integracaoField = value;
                    this.RaisePropertyChanged("status_integracao");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="KhanConsultasServiceReference.IConsultasService")]
    internal interface IConsultasService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConsultasService/ConsultarPedidosStatus", ReplyAction="http://tempuri.org/IConsultasService/ConsultarPedidosStatusResponse")]
        System.Collections.Generic.List<Glass.Integracao.Khan.KhanConsultasServiceReference.PedidoStatus> ConsultarPedidosStatus(System.Collections.Generic.List<string> args);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IConsultasService/ConsultarPedidosStatus", ReplyAction="http://tempuri.org/IConsultasService/ConsultarPedidosStatusResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<Glass.Integracao.Khan.KhanConsultasServiceReference.PedidoStatus>> ConsultarPedidosStatusAsync(System.Collections.Generic.List<string> args);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IConsultasServiceChannel : Glass.Integracao.Khan.KhanConsultasServiceReference.IConsultasService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class ConsultasServiceClient : System.ServiceModel.ClientBase<Glass.Integracao.Khan.KhanConsultasServiceReference.IConsultasService>, Glass.Integracao.Khan.KhanConsultasServiceReference.IConsultasService {
        
        public ConsultasServiceClient() {
        }
        
        public ConsultasServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ConsultasServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConsultasServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ConsultasServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Collections.Generic.List<Glass.Integracao.Khan.KhanConsultasServiceReference.PedidoStatus> ConsultarPedidosStatus(System.Collections.Generic.List<string> args) {
            return base.Channel.ConsultarPedidosStatus(args);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<Glass.Integracao.Khan.KhanConsultasServiceReference.PedidoStatus>> ConsultarPedidosStatusAsync(System.Collections.Generic.List<string> args) {
            return base.Channel.ConsultarPedidosStatusAsync(args);
        }
    }
}
