using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Glass.Log;

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis situações dos fornecedores do sistema.
    /// </summary>
    public enum SituacaoFornecedor
    {
        /// <summary>
        /// Ativo.
        /// </summary>
        [Description("Ativo")]
        [Display(Name = "Ativo")]
        Ativo = 1,
        /// <summary>
        /// Inativo.
        /// </summary>
        [Description("Inativo")]
        Inativo,
        /// <summary>
        /// Cancelado.
        /// </summary>
        [Description("Cancelado")]
        Cancelado,
        /// <summary>
        /// Bloqueado.
        /// </summary>
        [Description("Bloqueado")]
        Bloqueado
    }

    /// <summary>
    /// Possíveis regimes do fornecedor.
    /// </summary>
    public enum RegimeFornecedor
    {
        /// <summary>
        /// Regime normal.
        /// </summary>
        [Description("Regime Normal")]
        RegimeNormal = 1,
        /// <summary>
        /// Simples nacional.
        /// </summary>
        [Description("Simples Nacional")]
        SimplesNacional
    }

    [PersistenceBaseDAO(typeof(FornecedorDAO))]
	[PersistenceClass("fornecedor")]
	public class Fornecedor : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IFornecedor
    {
        #region Propriedades

        [PersistenceProperty("IDFORNEC", PersistenceParameterType.IdentityKey)]
        public int IdFornec { get; set; }

        [Log("Cidade", true, "NomeCidadeUf", typeof(CidadeDAO))]
        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidade { get; set; }

        [Log("Plano de Conta", "Descricao", typeof(PlanoContasDAO))]
        [PersistenceProperty("IDCONTA")]
        [PersistenceForeignKey(typeof(PlanoContas), "IdConta")]
        public int? IdConta { get; set; }

        [Log("Plano de Conta Contábil", "Descricao", typeof(PlanoContaContabilDAO))]
        [PersistenceProperty("IDCONTACONTABIL")]
        [PersistenceForeignKey(typeof(PlanoContaContabil), "IdContaContabil")]
        public int? IdContaContabil { get; set; }

        [Log("País", "NomePais", typeof(PaisDAO))]
        [PersistenceProperty("IDPAIS")]
        [PersistenceForeignKey(typeof(Pais), "IdPais")]
        public int IdPais { get; set; }

        [Log("Tipo de Pessoa")]
        [PersistenceProperty("TIPOPESSOA")]
        public string TipoPessoa { get; set; }

        [Log("Nome Fantasia", true)]
        [PersistenceProperty("NOMEFANTASIA")]
        public string Nomefantasia { get; set; }

        [Log("Razão Social")]
        [PersistenceProperty("RAZAOSOCIAL")]
        public string Razaosocial { get; set; }

        [Log("CPF/CNPJ", true)]
        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [PersistenceProperty("SITUACAO")]
        public SituacaoFornecedor Situacao { get; set; }

        [Log("RG/Inscrição Estadual", true)]
        [PersistenceProperty("RGINSCEST")]
        public string RgInscEst { get; set; }

        [Log("Produtor Rural", true)]
        [PersistenceProperty("PRODUTORRURAL")]
        public bool ProdutorRural { get; set; }

        [Log("Suframa", true)]
        [PersistenceProperty("SUFRAMA")]
        public string Suframa { get; set; }

        /// <summary>
        /// 1 - Regime Normal
        /// 2 - Simples Nacional
        /// </summary>
        [Log("CRT")]
        [PersistenceProperty("CRT")]
        public RegimeFornecedor Crt { get; set; }

        [Log("Endereço", true)]
        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [Log("Número", true)]
        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [Log("Complemento", true)]
        [PersistenceProperty("COMPL")]
        public string Compl { get; set; }

        [Log("Bairro", true)]
        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        [Log("CEP")]
        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [Log("E-mail")]
        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        [Log("Telefone de Contato")]
        [PersistenceProperty("TELCONT")]
        public string Telcont { get; set; }

        [Log("Fax")]
        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        [Log("Data da Última Compra")]
        [PersistenceProperty("DTULTCOMPRA")]
        public DateTime? Dtultcompra { get; set; }

        [Log("Data da vigência da tabela de preço.")]
        [PersistenceProperty("DATAVIGENCIAPRECOS")]
        public DateTime? DataVigenciaPrecos { get; set; }
         
        [Log("Crédito")]
        [PersistenceProperty("CREDITO")]
        public decimal Credito { get; set; }

        [Log("Vendedor")]
        [PersistenceProperty("VENDEDOR")]
        public string Vendedor { get; set; }

        [Log("Telefone Cel. Vendedor")]
        [PersistenceProperty("TELCELVEND")]
        public string Telcelvend { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Forma de Pagamento", "Descricao", typeof(ParcelasDAO))]
        [PersistenceProperty("TIPOPAGTO")]
        public int? TipoPagto { get; set; }

        [Log("Bloquear Pagamento")]
        [PersistenceProperty("BLOQUEARPAGTO")]
        public bool BloquearPagto { get; set; }

        /// <summary>
        /// Define o endereço do web service do cliente
        /// </summary>
        [Log("URL Sistema WebGlass")]
        [PersistenceProperty("URLSISTEMA")]
        public string UrlSistema { get; set; }

        [Log("Contato")]
        [PersistenceProperty("Contato")]
        public string Contato { get; set; }

        [Log("Email Contato")]
        [PersistenceProperty("EmailContato")]
        public string EmailContato { get; set; }

        [Log("Tel. Contato")]
        [PersistenceProperty("TelContato")]
        public string TelContato { get; set; }

        /// <summary>
        /// Define o documento fiscal do destinatário importação
        /// </summary>
        [Log("Passaporte / Doc")]
        [PersistenceProperty("PASSAPORTEDOC")]
        public string PassaporteDoc { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _cidade;

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string Cidade
        {
            get { return _cidade == null ? String.Empty : _cidade.ToUpper(); }
            set { _cidade = value; }
        }

        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string Uf { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DESCRPLANOCONTA", DirectionParameter.InputOptional)]
        public string DescrPlanoConta { get; set; }

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodIbgeUf { get; set; }

        [PersistenceProperty("NOMEPAIS", DirectionParameter.InputOptional)]
        public string NomePais { get; set; }

        [PersistenceProperty("PARCELA", DirectionParameter.InputOptional)]
        public string Parcela { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string EnderecoCompleto
        {
            get
            {
                string endereco = Endereco + ", " + Numero + " - " + Bairro;
                if (_cidade != null && Uf != null)
                    endereco += " - " + _cidade + "/" + Uf;

                endereco += " CEP: " + Cep;
                return endereco;
            }
        }

        public string CodMunicipio
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        public string Nome
        {
            get
            {
                return !String.IsNullOrEmpty(Nomefantasia) ? Nomefantasia :
                    !String.IsNullOrEmpty(Razaosocial) ? Razaosocial : "";
            }
        }

        public string IdNome
        {
            get { return IdFornec + " - " + Nome; }
        }

        public string CrtString
        {
            get
            {
                return Enum.GetName(typeof(RegimeFornecedor), Crt) == "SimplesNacional" ? "Simples Nacional" : "Regime Normal";
            }
        }

        [Log("Situação",true)]
        public string DescrSituacao
        {
            get
            {
                return Colosoft.Translator.Translate(Situacao).Format();
            }
        }

        // Apenas administrador pode inativar fornecedor pela lista
        public bool InativarVisible
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarFornecedor) && Situacao != SituacaoFornecedor.Cancelado;
            }
        }

        #endregion

        #region IFornecedor Members

        string Sync.Fiscal.EFD.Entidade.IFornecedor.RazaoSocial
        {
            get { return Razaosocial; }
        }

        #endregion

        #region IParticipanteNFe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Codigo
        {
            get { return IdFornec; }
        }

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CodigoCidade
        {
            get { return IdCidade ?? 0; }
        }

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IParticipanteNFe.TipoPessoa
        {
            get { return TipoPessoa == "F" ? Sync.Fiscal.Enumeracao.TipoPessoa.Fisica : Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.InscricaoEstadual
        {
            get { return RgInscEst; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Complemento
        {
            get { return Compl; }
        }

        #endregion
    }
}