using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(TransportadorDAO))]
	[PersistenceClass("transportador")]
	public class Transportador : Colosoft.Data.BaseModel, Sync.Fiscal.EFD.Entidade.ITransportador
    {
        #region Propriedades

        [PersistenceProperty("IDTRANSPORTADOR", PersistenceParameterType.IdentityKey)]
        public int IdTransportador { get; set; }

        [Log("Cidade", true, "NomeCidadeUf", typeof(CidadeDAO))]
        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int? IdCidade { get; set; }

        [Log("Nome", true)]
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [Log("Nome Fantasia", true)]
        [PersistenceProperty("NOMEFANTASIA")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// 1-Física
        /// 2-Jurídica
        /// </summary>
        [PersistenceProperty("TIPOPESSOA")]
        public int TipoPessoa { get; set; }

        [Log("CPF/CNPJ", true)]
        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [Log("Insc. Est.", true)]
        [PersistenceProperty("INSCEST")]
        public string InscEst { get; set; }

        [PersistenceProperty("PLACA")]
        public string Placa { get; set; }

		private string _endereco;

        [Log("Endereço", true)]
		[PersistenceProperty("ENDERECO")]
		public string Endereco
		{
			get { return _endereco != null ? _endereco : String.Empty; }
			set { _endereco = value; }
		}

		private string _numero;

        [Log("Numero", true)]
		[PersistenceProperty("NUMERO")]
		public string Numero
		{
			get { return _numero != null ? _numero : String.Empty; }
			set { _numero = value; }
		}

		private string _bairro;

        [Log("Bairro", true)]
		[PersistenceProperty("BAIRRO")]
		public string Bairro
		{
			get { return _bairro != null ? _bairro : String.Empty; }
			set { _bairro = value; }
		}

        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [PersistenceProperty("TELEFONE")]
        public string Telefone { get; set; }

        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        [PersistenceProperty("CONTATO")]
        public string Contato { get; set; }

        [PersistenceProperty("TELEFONECONTATO")]
        public string TelefoneContato { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("SUFRAMA", true)]
        [PersistenceProperty("SUFRAMA")]
        public string Suframa { get; set; }

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

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodIbgeUf { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string EnderecoNfe
        {
            get 
            {
                string endNfe = _endereco + " " + _numero + " - " + _bairro;

                return endNfe.Length > 60 ? endNfe.Substring(0, 60) : endNfe;
            }
        }

        public string CodMunicipio
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        #endregion

        #region Propriedade para Log

        internal DateTime DataCad { get; set; }

        #endregion

        #region IParticipanteNFe Members

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Codigo
        {
            get { return IdTransportador; }
        }

        int Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CodigoCidade
        {
            get { return IdCidade ?? 0; }
        }

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IParticipanteNFe.TipoPessoa
        {
            get { return TipoPessoa == 1 ? Sync.Fiscal.Enumeracao.TipoPessoa.Fisica : Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.CpfCnpj
        {
            get { return CpfCnpj; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.InscricaoEstadual
        {
            get { return InscEst; }
        }

        string Sync.Fiscal.EFD.Entidade.IParticipanteNFe.Complemento
        {
            get { return null; }
        }

        #endregion
    }
}