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

        [PersistenceProperty("IdContaR")]
        public uint IdContaR { get; set; }

        [PersistenceProperty("IdFormaPagto")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("ValorPagto")]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IdContaBanco")]
        public uint? IdContaBanco { get; set; }

        [PersistenceProperty("IdTipoCartao")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("IdDepositoNaoIdentificado")]
        public uint? IdDepositoNaoIdentificado { get; set; }

        [PersistenceProperty("NumAutCartao")]
        public string NumAutCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DescrFormaPagto", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        #endregion
    }
}
