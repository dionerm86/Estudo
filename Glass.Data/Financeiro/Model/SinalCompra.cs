using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SinalCompraDAO))]
    [PersistenceClass("sinal_compra")]
    public class SinalCompra
    {
        #region Construtores

        public SinalCompra(){}

        #endregion

        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Cancelado
        }

        #endregion

        #region Propriedades

        [Log("Num. Sinal")]
        [PersistenceProperty("IDSINALCOMPRA", PersistenceParameterType.IdentityKey)]
        public uint IdSinalCompra { get; set; }

        [Log("Fornecedor")]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [Log("Cancelado")]
        [PersistenceProperty("CANCELADO")]
        public bool Cancelado { get; set; }

        [Log("Compras")]
        [PersistenceProperty("IDSCOMPRAS")]
        public string IdsCompras { get; set; }

        [Log("Valores")]
        [PersistenceProperty("VALORES")]
        public string Valores { get; set; }

        [Log("Cheques")]
        [PersistenceProperty("IDSCHEQUES")]
        public string IdsCheques { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [Log("Data do Sinal")]
        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeFornecedor;

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornecedor
        {
            get { return _nomeFornecedor != null ? _nomeFornecedor.ToUpper() : String.Empty; }
            set { _nomeFornecedor = value; }
        }

        private decimal _totalSinal;

        [PersistenceProperty("TOTALSINAL", DirectionParameter.InputOptional)]
        public decimal TotalSinal
        {
            get { return _totalSinal; }
            set { _totalSinal = value; }
        }

        [PersistenceProperty("FORMAPAGTO", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        [PersistenceProperty("FUNCIONARIO", DirectionParameter.InputOptional)]
        public string Funcionario { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeFornecedor
        {
            get { return IdFornec + " - " + _nomeFornecedor; }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizadoCriar != null ? CreditoUtilizadoCriar.Value : 0;
                decimal gerado = CreditoGeradoCriar != null ? CreditoGeradoCriar.Value : 0;

                if (ValorCreditoAoCriar == null || (ValorCreditoAoCriar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoCriar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoCriar.Value - utilizado + gerado).ToString("C");
            }
        }

        public bool CancelarVisible
        {
            get { return !Cancelado; }
        }

        [Log("Retificar Sinal")]
        internal string DadosRetificar { get; set; }

        #endregion

    }
}
