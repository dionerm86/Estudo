﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Glass.Integracao.Khan.KhanProdutosServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Produtos", Namespace="http://schemas.datacontract.org/2004/07/KnIntegracao.Services")]
    [System.SerializableAttribute()]
    internal partial class Produtos : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CLASS_ABCField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODAPLICAField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODBARField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODCATEGField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODCLASSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODEMPRESAField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODMOEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CODTIPPROField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CodCADField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodFABField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodFAMField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodPROField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ESPECIFField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal EstoqField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal EstoqMAXField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal EstoqMINField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool FLAGVALIDOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool FlagmanField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool GERALOTEField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal IPIESPField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LETCLASSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal LUCROPROMOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal Lucro1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal Lucro2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MARCAField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NOCPROField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NORMAField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NomPROField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PCOFINSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PCUSTOFIXOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PDESCCOMField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PICMSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PIIMPField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PIPIField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PIRRFField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PISSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PPISField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PPerdaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PRCUSTREFERField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PRCUSTVARField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PREDICMSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PRPROMOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PRREFERField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PSEGSField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PesoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PrContabField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PrCustField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PrVend1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal PrVend2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal QTDEMBALAGEMField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SITTRIBField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TIPOPRODField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TokenField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UniPROField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string codempresa1Field;
        
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
        internal string CLASS_ABC {
            get {
                return this.CLASS_ABCField;
            }
            set {
                if ((object.ReferenceEquals(this.CLASS_ABCField, value) != true)) {
                    this.CLASS_ABCField = value;
                    this.RaisePropertyChanged("CLASS_ABC");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODAPLICA {
            get {
                return this.CODAPLICAField;
            }
            set {
                if ((object.ReferenceEquals(this.CODAPLICAField, value) != true)) {
                    this.CODAPLICAField = value;
                    this.RaisePropertyChanged("CODAPLICA");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODBAR {
            get {
                return this.CODBARField;
            }
            set {
                if ((object.ReferenceEquals(this.CODBARField, value) != true)) {
                    this.CODBARField = value;
                    this.RaisePropertyChanged("CODBAR");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODCATEG {
            get {
                return this.CODCATEGField;
            }
            set {
                if ((object.ReferenceEquals(this.CODCATEGField, value) != true)) {
                    this.CODCATEGField = value;
                    this.RaisePropertyChanged("CODCATEG");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODCLASS {
            get {
                return this.CODCLASSField;
            }
            set {
                if ((object.ReferenceEquals(this.CODCLASSField, value) != true)) {
                    this.CODCLASSField = value;
                    this.RaisePropertyChanged("CODCLASS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODEMPRESA {
            get {
                return this.CODEMPRESAField;
            }
            set {
                if ((object.ReferenceEquals(this.CODEMPRESAField, value) != true)) {
                    this.CODEMPRESAField = value;
                    this.RaisePropertyChanged("CODEMPRESA");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODMOE {
            get {
                return this.CODMOEField;
            }
            set {
                if ((object.ReferenceEquals(this.CODMOEField, value) != true)) {
                    this.CODMOEField = value;
                    this.RaisePropertyChanged("CODMOE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CODTIPPRO {
            get {
                return this.CODTIPPROField;
            }
            set {
                if ((object.ReferenceEquals(this.CODTIPPROField, value) != true)) {
                    this.CODTIPPROField = value;
                    this.RaisePropertyChanged("CODTIPPRO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal int CodCAD {
            get {
                return this.CodCADField;
            }
            set {
                if ((this.CodCADField.Equals(value) != true)) {
                    this.CodCADField = value;
                    this.RaisePropertyChanged("CodCAD");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CodFAB {
            get {
                return this.CodFABField;
            }
            set {
                if ((object.ReferenceEquals(this.CodFABField, value) != true)) {
                    this.CodFABField = value;
                    this.RaisePropertyChanged("CodFAB");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CodFAM {
            get {
                return this.CodFAMField;
            }
            set {
                if ((object.ReferenceEquals(this.CodFAMField, value) != true)) {
                    this.CodFAMField = value;
                    this.RaisePropertyChanged("CodFAM");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string CodPRO {
            get {
                return this.CodPROField;
            }
            set {
                if ((object.ReferenceEquals(this.CodPROField, value) != true)) {
                    this.CodPROField = value;
                    this.RaisePropertyChanged("CodPRO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string ESPECIF {
            get {
                return this.ESPECIFField;
            }
            set {
                if ((object.ReferenceEquals(this.ESPECIFField, value) != true)) {
                    this.ESPECIFField = value;
                    this.RaisePropertyChanged("ESPECIF");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal Estoq {
            get {
                return this.EstoqField;
            }
            set {
                if ((this.EstoqField.Equals(value) != true)) {
                    this.EstoqField = value;
                    this.RaisePropertyChanged("Estoq");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal EstoqMAX {
            get {
                return this.EstoqMAXField;
            }
            set {
                if ((this.EstoqMAXField.Equals(value) != true)) {
                    this.EstoqMAXField = value;
                    this.RaisePropertyChanged("EstoqMAX");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal EstoqMIN {
            get {
                return this.EstoqMINField;
            }
            set {
                if ((this.EstoqMINField.Equals(value) != true)) {
                    this.EstoqMINField = value;
                    this.RaisePropertyChanged("EstoqMIN");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal bool FLAGVALIDO {
            get {
                return this.FLAGVALIDOField;
            }
            set {
                if ((this.FLAGVALIDOField.Equals(value) != true)) {
                    this.FLAGVALIDOField = value;
                    this.RaisePropertyChanged("FLAGVALIDO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal bool Flagman {
            get {
                return this.FlagmanField;
            }
            set {
                if ((this.FlagmanField.Equals(value) != true)) {
                    this.FlagmanField = value;
                    this.RaisePropertyChanged("Flagman");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal bool GERALOTE {
            get {
                return this.GERALOTEField;
            }
            set {
                if ((this.GERALOTEField.Equals(value) != true)) {
                    this.GERALOTEField = value;
                    this.RaisePropertyChanged("GERALOTE");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal IPIESP {
            get {
                return this.IPIESPField;
            }
            set {
                if ((this.IPIESPField.Equals(value) != true)) {
                    this.IPIESPField = value;
                    this.RaisePropertyChanged("IPIESP");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string LETCLASS {
            get {
                return this.LETCLASSField;
            }
            set {
                if ((object.ReferenceEquals(this.LETCLASSField, value) != true)) {
                    this.LETCLASSField = value;
                    this.RaisePropertyChanged("LETCLASS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal LUCROPROMO {
            get {
                return this.LUCROPROMOField;
            }
            set {
                if ((this.LUCROPROMOField.Equals(value) != true)) {
                    this.LUCROPROMOField = value;
                    this.RaisePropertyChanged("LUCROPROMO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal Lucro1 {
            get {
                return this.Lucro1Field;
            }
            set {
                if ((this.Lucro1Field.Equals(value) != true)) {
                    this.Lucro1Field = value;
                    this.RaisePropertyChanged("Lucro1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal Lucro2 {
            get {
                return this.Lucro2Field;
            }
            set {
                if ((this.Lucro2Field.Equals(value) != true)) {
                    this.Lucro2Field = value;
                    this.RaisePropertyChanged("Lucro2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string MARCA {
            get {
                return this.MARCAField;
            }
            set {
                if ((object.ReferenceEquals(this.MARCAField, value) != true)) {
                    this.MARCAField = value;
                    this.RaisePropertyChanged("MARCA");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string NOCPRO {
            get {
                return this.NOCPROField;
            }
            set {
                if ((object.ReferenceEquals(this.NOCPROField, value) != true)) {
                    this.NOCPROField = value;
                    this.RaisePropertyChanged("NOCPRO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string NORMA {
            get {
                return this.NORMAField;
            }
            set {
                if ((object.ReferenceEquals(this.NORMAField, value) != true)) {
                    this.NORMAField = value;
                    this.RaisePropertyChanged("NORMA");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string NomPRO {
            get {
                return this.NomPROField;
            }
            set {
                if ((object.ReferenceEquals(this.NomPROField, value) != true)) {
                    this.NomPROField = value;
                    this.RaisePropertyChanged("NomPRO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PCOFINS {
            get {
                return this.PCOFINSField;
            }
            set {
                if ((this.PCOFINSField.Equals(value) != true)) {
                    this.PCOFINSField = value;
                    this.RaisePropertyChanged("PCOFINS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PCUSTOFIXO {
            get {
                return this.PCUSTOFIXOField;
            }
            set {
                if ((this.PCUSTOFIXOField.Equals(value) != true)) {
                    this.PCUSTOFIXOField = value;
                    this.RaisePropertyChanged("PCUSTOFIXO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PDESCCOM {
            get {
                return this.PDESCCOMField;
            }
            set {
                if ((this.PDESCCOMField.Equals(value) != true)) {
                    this.PDESCCOMField = value;
                    this.RaisePropertyChanged("PDESCCOM");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PICMS {
            get {
                return this.PICMSField;
            }
            set {
                if ((this.PICMSField.Equals(value) != true)) {
                    this.PICMSField = value;
                    this.RaisePropertyChanged("PICMS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PIIMP {
            get {
                return this.PIIMPField;
            }
            set {
                if ((this.PIIMPField.Equals(value) != true)) {
                    this.PIIMPField = value;
                    this.RaisePropertyChanged("PIIMP");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PIPI {
            get {
                return this.PIPIField;
            }
            set {
                if ((this.PIPIField.Equals(value) != true)) {
                    this.PIPIField = value;
                    this.RaisePropertyChanged("PIPI");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PIRRF {
            get {
                return this.PIRRFField;
            }
            set {
                if ((this.PIRRFField.Equals(value) != true)) {
                    this.PIRRFField = value;
                    this.RaisePropertyChanged("PIRRF");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PISS {
            get {
                return this.PISSField;
            }
            set {
                if ((this.PISSField.Equals(value) != true)) {
                    this.PISSField = value;
                    this.RaisePropertyChanged("PISS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PPIS {
            get {
                return this.PPISField;
            }
            set {
                if ((this.PPISField.Equals(value) != true)) {
                    this.PPISField = value;
                    this.RaisePropertyChanged("PPIS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PPerda {
            get {
                return this.PPerdaField;
            }
            set {
                if ((this.PPerdaField.Equals(value) != true)) {
                    this.PPerdaField = value;
                    this.RaisePropertyChanged("PPerda");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PRCUSTREFER {
            get {
                return this.PRCUSTREFERField;
            }
            set {
                if ((this.PRCUSTREFERField.Equals(value) != true)) {
                    this.PRCUSTREFERField = value;
                    this.RaisePropertyChanged("PRCUSTREFER");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PRCUSTVAR {
            get {
                return this.PRCUSTVARField;
            }
            set {
                if ((this.PRCUSTVARField.Equals(value) != true)) {
                    this.PRCUSTVARField = value;
                    this.RaisePropertyChanged("PRCUSTVAR");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PREDICMS {
            get {
                return this.PREDICMSField;
            }
            set {
                if ((this.PREDICMSField.Equals(value) != true)) {
                    this.PREDICMSField = value;
                    this.RaisePropertyChanged("PREDICMS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PRPROMO {
            get {
                return this.PRPROMOField;
            }
            set {
                if ((this.PRPROMOField.Equals(value) != true)) {
                    this.PRPROMOField = value;
                    this.RaisePropertyChanged("PRPROMO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PRREFER {
            get {
                return this.PRREFERField;
            }
            set {
                if ((this.PRREFERField.Equals(value) != true)) {
                    this.PRREFERField = value;
                    this.RaisePropertyChanged("PRREFER");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PSEGS {
            get {
                return this.PSEGSField;
            }
            set {
                if ((this.PSEGSField.Equals(value) != true)) {
                    this.PSEGSField = value;
                    this.RaisePropertyChanged("PSEGS");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal Peso {
            get {
                return this.PesoField;
            }
            set {
                if ((this.PesoField.Equals(value) != true)) {
                    this.PesoField = value;
                    this.RaisePropertyChanged("Peso");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PrContab {
            get {
                return this.PrContabField;
            }
            set {
                if ((this.PrContabField.Equals(value) != true)) {
                    this.PrContabField = value;
                    this.RaisePropertyChanged("PrContab");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PrCust {
            get {
                return this.PrCustField;
            }
            set {
                if ((this.PrCustField.Equals(value) != true)) {
                    this.PrCustField = value;
                    this.RaisePropertyChanged("PrCust");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PrVend1 {
            get {
                return this.PrVend1Field;
            }
            set {
                if ((this.PrVend1Field.Equals(value) != true)) {
                    this.PrVend1Field = value;
                    this.RaisePropertyChanged("PrVend1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal PrVend2 {
            get {
                return this.PrVend2Field;
            }
            set {
                if ((this.PrVend2Field.Equals(value) != true)) {
                    this.PrVend2Field = value;
                    this.RaisePropertyChanged("PrVend2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal decimal QTDEMBALAGEM {
            get {
                return this.QTDEMBALAGEMField;
            }
            set {
                if ((this.QTDEMBALAGEMField.Equals(value) != true)) {
                    this.QTDEMBALAGEMField = value;
                    this.RaisePropertyChanged("QTDEMBALAGEM");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string SITTRIB {
            get {
                return this.SITTRIBField;
            }
            set {
                if ((object.ReferenceEquals(this.SITTRIBField, value) != true)) {
                    this.SITTRIBField = value;
                    this.RaisePropertyChanged("SITTRIB");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string TIPOPROD {
            get {
                return this.TIPOPRODField;
            }
            set {
                if ((object.ReferenceEquals(this.TIPOPRODField, value) != true)) {
                    this.TIPOPRODField = value;
                    this.RaisePropertyChanged("TIPOPROD");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Token {
            get {
                return this.TokenField;
            }
            set {
                if ((object.ReferenceEquals(this.TokenField, value) != true)) {
                    this.TokenField = value;
                    this.RaisePropertyChanged("Token");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string UniPRO {
            get {
                return this.UniPROField;
            }
            set {
                if ((object.ReferenceEquals(this.UniPROField, value) != true)) {
                    this.UniPROField = value;
                    this.RaisePropertyChanged("UniPRO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(Name="codempresa")]
        internal string codempresa1 {
            get {
                return this.codempresa1Field;
            }
            set {
                if ((object.ReferenceEquals(this.codempresa1Field, value) != true)) {
                    this.codempresa1Field = value;
                    this.RaisePropertyChanged("codempresa1");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="KhanProdutosServiceReference.IProdutosService")]
    internal interface IProdutosService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProdutosService/SalvarProdutos", ReplyAction="http://tempuri.org/IProdutosService/SalvarProdutosResponse")]
        Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos SalvarProdutos(Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos prod);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProdutosService/ConsultarProdutos", ReplyAction="http://tempuri.org/IProdutosService/ConsultarProdutosResponse")]
        System.Collections.Generic.List<Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos> ConsultarProdutos(string CodPRO);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal interface IProdutosServiceChannel : Glass.Integracao.Khan.KhanProdutosServiceReference.IProdutosService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    internal partial class ProdutosServiceClient : System.ServiceModel.ClientBase<Glass.Integracao.Khan.KhanProdutosServiceReference.IProdutosService>, Glass.Integracao.Khan.KhanProdutosServiceReference.IProdutosService {
        
        public ProdutosServiceClient() {
        }
        
        public ProdutosServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProdutosServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProdutosServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProdutosServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos SalvarProdutos(Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos prod) {
            return base.Channel.SalvarProdutos(prod);
        }
        
        public System.Collections.Generic.List<Glass.Integracao.Khan.KhanProdutosServiceReference.Produtos> ConsultarProdutos(string CodPRO) {
            return base.Channel.ConsultarProdutos(CodPRO);
        }
    }
}
