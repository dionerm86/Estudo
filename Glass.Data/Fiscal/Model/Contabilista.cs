using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContabilistaDAO))]
    [PersistenceClass("contabilista")]
    public class Contabilista : ModelBaseCadastro, Sync.Fiscal.EFD.Entidade.IContabilista
    {
        #region Propriedades

        [PersistenceProperty("IDCONTABILISTA", PersistenceParameterType.IdentityKey)]
        public uint IdContabilista { get; set; }

        [PersistenceProperty("IDCIDADE")]
        public uint IdCidade { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("TIPOPESSOA")]
        public string TipoPessoa { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [PersistenceProperty("CRC")]
        public string Crc { get; set; }

        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [PersistenceProperty("COMPL")]
        public string Compl { get; set; }

        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        [PersistenceProperty("TELCONT")]
        public string TelCont { get; set; }

        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string NomeCidade { get; set; }

        [PersistenceProperty("CODIBGECIDADE", DirectionParameter.InputOptional)]
        public string CodIbgeCidade { get; set; }

        [PersistenceProperty("CODIBGEUF", DirectionParameter.InputOptional)]
        public string CodIbgeUf { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string CodIbgeMunicipio
        {
            get { return CodIbgeUf + CodIbgeCidade; }
        }

        public string EnderecoCompleto
        {
            get
            {
                string compl = String.IsNullOrEmpty(Compl) ? " " : " (" + Compl + ") ";

                return Endereco + (!String.IsNullOrEmpty(Numero) ? ", " + Numero : String.Empty) + compl + Bairro + " - " + NomeCidade;
            }
        }

        #endregion

        #region IContabilista Members

        Sync.Fiscal.Enumeracao.TipoPessoa Sync.Fiscal.EFD.Entidade.IContabilista.TipoPessoa
        {
            get { return TipoPessoa == "F" ? Sync.Fiscal.Enumeracao.TipoPessoa.Fisica : Sync.Fiscal.Enumeracao.TipoPessoa.Juridica; }
        }

        string Sync.Fiscal.EFD.Entidade.IContabilista.CRC
        {
            get { return Crc; }
        }

        string Sync.Fiscal.EFD.Entidade.IContabilista.CEP
        {
            get { return Cep; }
        }

        string Sync.Fiscal.EFD.Entidade.IContabilista.Complemento
        {
            get { return Compl; }
        }

        string Sync.Fiscal.EFD.Entidade.IContabilista.TelefoneContato
        {
            get { return TelCont; }
        }

        int Sync.Fiscal.EFD.Entidade.IContabilista.CodigoCidade
        {
            get { return (int)IdCidade; }
        }

        #endregion
    }
}