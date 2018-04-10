using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoContasReceberDAO))]
    [PersistenceClass("Pagto_Contas_Receber")]
    public class PagtoContasReceber
    {
        #region Propriedades

        [PersistenceProperty("IdPagtoContasReceber", PersistenceParameterType.IdentityKey)]
        public int IdPagtoContasReceber { get; set; }

        [PersistenceProperty("IDCONTAR")]
        public uint IdContaR { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALORPAGTO")]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("IDDEPOSITONAOIDENTIFICADO")]
        public uint? IdDepositoNaoIdentificado { get; set; }

        [PersistenceProperty("IDCARTAONAOIDENTIFICADO")]
        public int? IdCartaoNaoIdentificado { get; set; }

        [PersistenceProperty("QUANTIDADEPARCELACARTAO")]
        public int? QuantidadeParcelaCartao { get; set; }

        [PersistenceProperty("TAXAANTECIPACAO")]
        public decimal? TaxaAntecipacao { get; set; }

        [PersistenceProperty("TIPOBOLETO")]
        public int? TipoBoleto { get; set; }

        [PersistenceProperty("NUMAUTCARTAO")]
        public string NumAutCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescrFormaPagto", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        #endregion
    }
}
