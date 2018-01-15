using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ItemAcertoChequeDAO))]
    [PersistenceClass("item_acerto_cheque")]
    public class ItemAcertoCheque
    {
        #region Propriedades

        [PersistenceProperty("IDITEMACERTOCHEQUE", PersistenceParameterType.IdentityKey)]
        public uint IdItemAcertoCheque { get; set; }

        [PersistenceProperty("IDACERTOCHEQUE")]
        public uint IdAcertoCheque { get; set; }

        [PersistenceProperty("IDCHEQUE")]
        public uint IdCheque { get; set; }

        [PersistenceProperty("VALORRECEB")]
        public decimal ValorReceb { get; set; }

        #endregion
    }
}