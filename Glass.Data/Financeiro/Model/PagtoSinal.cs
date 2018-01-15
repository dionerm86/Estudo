using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoSinalDAO))]
    [PersistenceClass("pagto_sinal")]
    public class PagtoSinal
    {
        #region Propriedades

        [PersistenceProperty("IDSINAL", PersistenceParameterType.Key)]
        public uint IdSinal { get; set; }

        [PersistenceProperty("NUMFORMAPAGTO", PersistenceParameterType.Key)]
        public int NumFormaPagto { get; set; }

        [PersistenceProperty("IDFORMAPAGTO")]
        public uint IdFormaPagto { get; set; }

        [PersistenceProperty("VALORPAGTO", PersistenceParameterType.Key)]
        public decimal ValorPagto { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

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