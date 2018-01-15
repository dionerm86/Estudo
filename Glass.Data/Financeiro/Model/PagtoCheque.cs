using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoChequeDAO))]
    [PersistenceClass("pagto_cheque")]
    public class PagtoCheque
    {
        #region Propriedades

        [PersistenceProperty("IDPAGTOCHEQUE", PersistenceParameterType.IdentityKey)]
        public uint IdPagtoCheque { get; set; }

        [PersistenceProperty("IDPAGTO")]
        public uint IdPagto { get; set; }

        [PersistenceProperty("IDSINALCOMPRA")]
        public uint? IdSinalCompra { get; set; }

        [PersistenceProperty("IDANTECIPFORNEC")]
        public uint? IdAntecipFornec { get; set; }

        [PersistenceProperty("IDCHEQUE")]
        public uint IdCheque { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint? IdContaBanco { get; set; }

        #endregion
    }
}