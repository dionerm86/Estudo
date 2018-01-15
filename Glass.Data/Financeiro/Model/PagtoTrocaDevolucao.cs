using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoTrocaDevolucaoDAO))]
    [PersistenceClass("pagto_troca_dev")]
    public class PagtoTrocaDevolucao
    {
        #region Propriedades

        [PersistenceProperty("IDPAGTOTROCADEV", PersistenceParameterType.IdentityKey)]
        public uint IdPagtoTrocaDev { get; set; }

        [PersistenceProperty("IDTROCADEVOLUCAO")]
        public uint IdTrocaDevolucao { get; set; }

        [PersistenceProperty("NUMFORMAPAGTO")]
        public int NumFormaPagto { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALORPAGO")]
        public decimal ValorPago { get; set; }

        [PersistenceProperty("IDTIPOCARTAO")]
        public uint? IdTipoCartao { get; set; }

        [PersistenceProperty("NumAutCartao")]
        public string NumAutCartao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRFORMAPAGTO", DirectionParameter.InputOptional)]
        public string DescrFormaPagto { get; set; }

        #endregion
    }
}