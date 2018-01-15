using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ComissionadoDAO))]
    [PersistenceClass("comissionado")]
    public class Comissionado : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCOMISSIONADO", PersistenceParameterType.IdentityKey)]
        public int IdComissionado { get; set; }

        [PersistenceProperty("IDCIDADE")]
        [PersistenceForeignKey(typeof(Cidade), "IdCidade")]
        public int IdCidade { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("TIPOPESSOA")]
        public string TipoPessoa { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("ENDERECO")]
        public string Endereco { get; set; }

        [PersistenceProperty("NUMERO")]
        public string Numero { get; set; }

        [PersistenceProperty("COMPL")]
        public string Compl { get; set; }

        [PersistenceProperty("BAIRRO")]
        public string Bairro { get; set; }

        [PersistenceProperty("CIDADE")]
        public string Cidade { get; set; }

        [PersistenceProperty("CEP")]
        public string Cep { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [PersistenceProperty("RGINSCEST")]
        public string RgInscEst { get; set; }

        [PersistenceProperty("TELRES")]
        public string TelRes { get; set; }

        [PersistenceProperty("TELCONT")]
        public string TelCont { get; set; }

        [PersistenceProperty("TELCEL")]
        public string TelCel { get; set; }

        [PersistenceProperty("FAX")]
        public string Fax { get; set; }

        [PersistenceProperty("EMAIL")]
        public string Email { get; set; }

        private string _obs;

        [PersistenceProperty("OBS")]
        public string Obs
        {
            get { return _obs != null ? _obs : String.Empty; }
            set { _obs = value; }
        }

        [PersistenceProperty("PERCENTUAL")]
        public float Percentual { get; set; }

        [PersistenceProperty("BANCO")]
        public string Banco { get; set; }

        [PersistenceProperty("AGENCIA")]
        public string Agencia { get; set; }

        [PersistenceProperty("CONTA")]
        public string Conta { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeCidade;

        [PersistenceProperty("NOMECIDADE", DirectionParameter.InputOptional)]
        public string NomeCidade
        {
            get { return _nomeCidade == null ? String.Empty : _nomeCidade.ToUpper(); }
            set { _nomeCidade = value; }
        }

        [PersistenceProperty("UF", DirectionParameter.InputOptional)]
        public string Uf { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string EnderecoCompleto
        {
            get
            {
                string _compl = String.IsNullOrEmpty(Compl) ? " " : " (" + Compl + ") ";

                return Endereco + (!String.IsNullOrEmpty(Numero) ? ", " + Numero : String.Empty) + _compl + Bairro + " - " + Cidade + "/" + Uf;
            }
        }

        #endregion
    }
}