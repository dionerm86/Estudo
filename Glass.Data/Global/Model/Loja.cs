using System;
using GDA;
using System.Xml.Serialization;
using Glass.Data.DAL;
using Glass.Log;
using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Enumeração com os tipos de loja.
    /// </summary>
    public enum TipoLoja
    {
        /// <summary>
        /// Comércio.
        /// </summary>
        [Description("Comércio")]
        Comercio = 1,
        /// <summary>
        /// Indústria.
        /// </summary>
        [Description("Indústria")]
        Industria
    }

    /// <summary>
    /// Enumeração com os CRT possíveis para a loja.
    /// </summary>
    public enum CrtLoja
    {
        /// <summary>
        /// Simples Nacional.
        /// </summary>
        [Description("Simples Nacional")]
        SimplesNacional = 1,
        /// <summary>
        /// Simples Nacional - excesso de sublimite de receita bruta.
        /// </summary>
        [Description("Simples Nacional - excesso de sublimite de receita bruta")]
        SimplesNacionalExcSub,
        /// <summary>
        /// Lucro Real.
        /// </summary>
        [Description("Lucro Real")]
        LucroReal,
        /// <summary>
        /// Lucro Presumido.
        /// </summary>
        [Description("Lucro Presumido")]
        LucroPresumido
    }

    [PersistenceBaseDAO(typeof(LojaDAO))]
    [PersistenceClass("loja")]
    [Colosoft.Data.Schema.Cache]
	public class Loja : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.ILoja
    {
        #region Propriedades

        [PersistenceProperty("IdLoja", PersistenceParameterType.IdentityKey)]
        public int IdLoja { get; set; }

        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidade { get; set; }

        [PersistenceProperty("RAZAOSOCIAL")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string RazaoSocial { get; set; }

        [Log("Nome Fantasia", true)]
        [PersistenceProperty("NOMEFANTASIA")]
        [Colosoft.Data.Schema.CacheIndexed]
        public string NomeFantasia { get; set; }
        
        /// <summary>
        /// 1-Ativa
        /// 2-Inativa
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        [Colosoft.Data.Schema.CacheIndexed]
        [XmlIgnore]
        public Glass.Situacao Situacao { get; set; }

        [XmlElement(ElementName = "Situacao")]
        public int SituacaoInt
        {
            get { return (int)Situacao; }
            set { Situacao = (Glass.Situacao)value; }
        }

        [PersistenceProperty("IGNORARBLOQUEARITENSCORESPESSURA")]
        [Colosoft.Data.Schema.CacheIndexed]
        [XmlIgnore]
        public bool IgnorarBloquearItensCorEspessura { get; set; }

        [PersistenceProperty("IGNORARLIBERARAPENASPRODUTOSPRONTOS")]
        [Colosoft.Data.Schema.CacheIndexed]
        [XmlIgnore]
        public bool IgnorarLiberarApenasProdutosProntos { get; set; }

        /// <summary>
        /// 1-Comércio
        /// 2-Indústria
        /// </summary>
        [Log("Tipo")]
        [PersistenceProperty("TIPO")]
        [Colosoft.Data.Schema.CacheIndexed]
        [XmlIgnore]
        public TipoLoja? Tipo { get; set; }

        [XmlElement(ElementName = "Tipo")]
        public int TipoInt 
        {
            get { return (int)Tipo; }
            set { Tipo = (TipoLoja)value; }
        }


        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [PersistenceProperty("COMPL")]
        public string Compl { get; set; }

        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [PersistenceProperty("SITE")]
        public string Site { get; set; }

        [PersistenceProperty("TELEFONE")]
        public string Telefone { get; set; }

        [PersistenceProperty("TELEFONE2")]
        public string Telefone2 { get; set; }

        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        [PersistenceProperty("CNPJ")]
        public string Cnpj { get; set; }

        [PersistenceProperty("INSCEST")]
        public string InscEst { get; set; }

        [PersistenceProperty("INSCESTST")]
        public string InscEstSt { get; set; }

        [PersistenceProperty("INSCMUNIC")]
        public string InscMunic { get; set; }

        [PersistenceProperty("CNAE")]
        public string Cnae { get; set; }

        [PersistenceProperty("SUFRAMA")]
        public string Suframa { get; set; }

        /// <summary>
        /// Código de Remige Tributário
        /// 1-Simples Nacional
        /// 2-Simples Nacional - excesso de sublimite de receita bruta
        /// 3-Lucro Real (Regime Normal na NFe)
        /// 4-Lucro Presumido (Não existe na NFe)
        /// </summary>
        [PersistenceProperty("CRT")]
        public int? Crt { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SENHACERT")]
        public string SenhaCert { get; set; }

        [XmlIgnore]
        [PersistenceProperty("EMAILFISCAL")]
        public string EmailFiscal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("LOGINEMAILFISCAL")]
        public string LoginEmailFiscal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SENHAEMAILFISCAL")]
        public string SenhaEmailFiscal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SERVIDOREMAILFISCAL")]
        public string ServidorEmailFiscal { get; set; }

        [XmlIgnore]
        [PersistenceProperty("EMAILCONTATO")]
        public string EmailContato { get; set; }

        [XmlIgnore]
        [PersistenceProperty("LOGINEMAILCONTATO")]
        public string LoginEmailContato { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SENHAEMAILCONTATO")]
        public string SenhaEmailContato { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SERVIDOREMAILCONTATO")]
        public string ServidorEmailContato { get; set; }

        [XmlIgnore]
        [PersistenceProperty("EMAILCOMERCIAL")]
        public string EmailComercial { get; set; }

        [XmlIgnore]
        [PersistenceProperty("LOGINEMAILCOMERCIAL")]
        public string LoginEmailComercial { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SENHAEMAILCOMERCIAL")]
        public string SenhaEmailComercial { get; set; }

        [XmlIgnore]
        [PersistenceProperty("SERVIDOREMAILCOMERCIAL")]
        public string ServidorEmailComercial { get; set; }

        [PersistenceProperty("REGIMELOJA")]
        public int? RegimeLoja { get; set; }

        [PersistenceProperty("TIPOATIVIDADE")]
        public int? TipoAtividade { get; set; }

        [PersistenceProperty("DATAVENCIMENTOCERTIFICADO")]
        public DateTime? DataVencimentoCertificado { get; set; }

        /// <summary>
        /// Número de inscrição na junta comercial.
        /// </summary>
        [PersistenceProperty("NIRE")]
        public string NIRE { get; set; }

        /// <summary>
        /// Data do número de inscrição na junta comercial.
        /// </summary>
        [PersistenceProperty("DATANIRE")]
        public DateTime? DataNIRE { get; set; }

        [PersistenceProperty("RNTRC")]
        public string RNTRC { get; set; }

        [PersistenceProperty("IdCsc")]
        public string IdCsc { get; set; }

        [PersistenceProperty("Csc")]
        public string Csc { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _cidade;

        [XmlIgnore]
        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string Cidade
        {
            get { return _cidade == null ? String.Empty : _cidade.ToUpper(); }
            set { _cidade = value; }
        }

        [XmlIgnore]
        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string Uf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodUf { get; set; }

        [XmlIgnore]
        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        #endregion

        #region Propriedades de Suporte

        [XmlIgnore]
        public string CodIBGECompleto
        {
            get { return CodUf + CodIbgeCidade; }
        }

        [XmlIgnore]
        public string DescrEndereco
        {
            get 
            { 
                return Endereco + ", " + Bairro + (!String.IsNullOrEmpty(Compl) ? "(" + Compl + ")" : String.Empty) + 
                    " - " + _cidade + "/" + Uf + " " + Cep; 
            }
        }

        [XmlIgnore]
        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                return Colosoft.Translator.Translate((Situacao)Situacao, "fem").Format();
            }
        }

        #endregion

        #region ILoja Members

        uint Sync.Fiscal.EFD.Entidade.ILoja.IdLoja
        {
            get { return (uint)IdLoja; }
        }

        string Sync.Fiscal.EFD.Entidade.ILoja.InscricaoMunicipal
        {
            get { return InscMunic; }
        }

        string Sync.Fiscal.EFD.Entidade.ILoja.Email
        {
            get { return Site; }
        }

        bool Sync.Fiscal.EFD.Entidade.ILoja.Simples
        {
            get { return Crt == (int)CrtLoja.SimplesNacional || Crt == (int)CrtLoja.SimplesNacionalExcSub; }
        }

        bool Sync.Fiscal.EFD.Entidade.ILoja.LucroReal
        {
            get { return Crt == (int)CrtLoja.LucroReal; }
        }

        bool Sync.Fiscal.EFD.Entidade.ILoja.LucroPresumido
        {
            get { return Crt == (int)CrtLoja.LucroPresumido; }
        }

        Sync.Fiscal.Enumeracao.Loja.RegimeLoja? Sync.Fiscal.EFD.Entidade.ILoja.RegimeLoja
        {
            get { return (Sync.Fiscal.Enumeracao.Loja.RegimeLoja)RegimeLoja; }
        }

        Sync.Fiscal.Enumeracao.Loja.TipoAtividadeContribuicoes? Sync.Fiscal.EFD.Entidade.ILoja.TipoAtividade
        {
            get { return (Sync.Fiscal.Enumeracao.Loja.TipoAtividadeContribuicoes?)TipoAtividade; }
        }

        #endregion

        #region IParticipanteNFe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Codigo
        {
            get { return IdLoja; }
        }

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CodigoCidade
        {
            get { return IdCidade ?? 0; }
        }

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IParticipanteNFe.TipoPessoa
        {
            get { return Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CpfCnpj
        {
            get { return Cnpj; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.InscricaoEstadual
        {
            get { return InscEst; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Complemento
        {
            get { return Compl; }
        }

        #endregion
    }
}