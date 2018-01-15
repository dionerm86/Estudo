using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LimiteChequeCpfCnpjDAO))]
    [PersistenceClass("limite_cheque_cpfcnpj")]
    public class LimiteChequeCpfCnpj
    {
        #region Propriedades

        [PersistenceProperty("IDLIMITECHEQUE", PersistenceParameterType.IdentityKey)]
        public uint IdLimiteCheque { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }

        [Log("Limite")]
        [PersistenceProperty("LIMITE")]
        public decimal Limite { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        #endregion
    }
}
