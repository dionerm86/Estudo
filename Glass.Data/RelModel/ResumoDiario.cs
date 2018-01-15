using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ResumoDiarioDAO))]
    [PersistenceClass("resumo_diario")]
    public class ResumoDiario
    {
        #region Propriedades

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public int? IdCliente { get; set; }

        [PersistenceProperty("Data", DirectionParameter.InputOptional)]
        public DateTime Data { get; set; }

        [PersistenceProperty("ChequeVista")]
        public decimal ChequeVista { get; set; }

        [PersistenceProperty("ChequePrazo")]
        public decimal ChequePrazo { get; set; }

        [PersistenceProperty("Dinheiro")]
        public decimal Dinheiro { get; set; }

        [PersistenceProperty("CartaoDebito")]
        public decimal CartaoDebito { get; set; }

        [PersistenceProperty("CartaoCredito")]
        public decimal CartaoCredito { get; set; }

        [PersistenceProperty("Credito")]
        public decimal Credito { get; set; }

        [PersistenceProperty("CreditoGerado")]
        public decimal CreditoGerado { get; set; }

        [PersistenceProperty("Troca")]
        public decimal Troca { get; set; }

        [PersistenceProperty("PagtoChequeDevolvido")]
        public decimal PagtoChequeDevolvido { get; set; }

        [PersistenceProperty("PagtoChequeDevolvidoDinheiro")]
        public decimal PagtoChequeDevolvidoDinheiro { get; set; }

        [PersistenceProperty("PagtoChequeDevolvidoCheque")]
        public decimal PagtoChequeDevolvidoCheque { get; set; }

        [PersistenceProperty("PagtoChequeDevolvidoOutros")]
        public decimal PagtoChequeDevolvidoOutros { get; set; }

        [PersistenceProperty("Boleto")]
        public decimal Boleto { get; set; }

        [PersistenceProperty("Construcard")]
        public decimal Construcard { get; set; }

        [PersistenceProperty("NotaPromissoria")]
        public decimal NotaPromissoria { get; set; }

        [PersistenceProperty("PagtoNotaPromissoria")]
        public decimal PagtoNotaPromissoria { get; set; }

        [PersistenceProperty("Deposito")]
        public decimal Deposito { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NomeCliente
        {
            get
            {
                if (IdCliente > 0)
                    return DAL.ClienteDAO.Instance.GetNome((uint)IdCliente.Value);
                else
                    return string.Empty;
            }
        }

        public string IdNomeCliente
        {
            get
            {
                if (IdCliente > 0)
                    return string.Format("{0} - {1}", IdCliente, NomeCliente);
                else
                    return string.Empty;
            }
        }

        public decimal Total
        {
            get { return ChequeVista + ChequePrazo + Dinheiro + CartaoDebito + CartaoCredito + Credito + Troca + Boleto + Deposito; }
        }

        #endregion
    }
}