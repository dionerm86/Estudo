using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoAcertoChequeDAO))]
    [PersistenceClass("pagto_acerto_cheque")]
    public class PagtoAcertoCheque
    {
        #region Propriedades

        [PersistenceProperty("IDACERTOCHEQUE", PersistenceParameterType.Key)]
        public uint IdAcertoCheque { get; set; }

        [PersistenceProperty("NUMFORMAPAGTO", PersistenceParameterType.Key)]
        public int NumFormaPagto { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALORPAGTO")]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IDDEPOSITONAOIDENTIFICADO")]
        public int? IdDepositoNaoIdentificado { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public int? IdCartaoNaoIdentificado { get; set; }

        [PersistenceProperty("QUANTIDADEPARCELACARTAO")]
        public int? QuantidadeParcelaCartao { get; set; }

        [PersistenceProperty("NUMAUTCARTAO")]
        public string NumAutCartao { get; set; }

        #endregion
    }
}