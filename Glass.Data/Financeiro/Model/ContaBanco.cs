using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContaBancoDAO))]
	[PersistenceClass("conta_banco")]
	public class ContaBanco : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDCONTABANCO", PersistenceParameterType.IdentityKey)]
        public int IdContaBanco { get; set; }

        [Log("Loja", "Nome", typeof(LojaDAO))]
        [PersistenceProperty("IDLOJA")]
        [PersistenceForeignKey(typeof(Loja), "IdLoja")]
        public int IdLoja { get; set; }

        [Log("Cód. do Banco")]
        [PersistenceProperty("CODBANCO")]
        public int? CodBanco { get; set; }

        [Log("Cód. do Convenio")]
        [PersistenceProperty("CODCONVENIO")]
        public string CodConvenio { get; set; }

		private string _nome;

        [Log("Banco")]
		[PersistenceProperty("NOME")]
		public string Nome
		{
            get { return _nome != null ? _nome : String.Empty; }
			set { _nome = value; }
		}

		private string _agencia;

        [Log("Agencia")]
		[PersistenceProperty("AGENCIA")]
		public string Agencia
		{
            get { return _agencia != null ? _agencia : String.Empty; }
			set { _agencia = value; }
		}

		private string _conta;

        [Log("Conta")]
		[PersistenceProperty("CONTA")]
		public string Conta
		{
			get { return _conta != null ? _conta : String.Empty; }
			set { _conta = value; }
		}

        [Log("Posto")]
        [PersistenceProperty("POSTO")]
        public int? Posto { get; set; }

        [Log("Titular")]
        [PersistenceProperty("TITULAR")]
        public string Titular { get; set; }

        /// <summary>
        /// 1-Ativa
        /// 2-Inativa
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [Log("Cód. Cliente")]
        [PersistenceProperty("CodCliente")]
        public string CodCliente { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMELOJA", DirectionParameter.InputOptional)]
        public string NomeLoja { get; set; }

        [PersistenceProperty("PODEEDITAR", DirectionParameter.InputOptional)]
        public bool PodeEditar { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DataSaldo { get; set; }

        public string Descricao
        {
            get
            {
                return _nome + " Agência: " + _agencia + " Conta: " + _conta;
            }
        }

        public string DescricaoSaldo
        {
            get { return Descricao + " (Saldo " + Saldo.ToString("C") + ")"; }
        }

        public string DescrSituacao
        {
            get { return Colosoft.Translator.Translate(Situacao).Format(); }
        }

        public decimal Saldo
        {
            get { return MovBancoDAO.Instance.GetSaldo((uint)IdContaBanco, DataSaldo, false); }
        }

        public decimal SaldoSemCheques
        {
            get { return MovBancoDAO.Instance.GetSaldo((uint)IdContaBanco, DataSaldo, true); }
        }

        #endregion
    }
}