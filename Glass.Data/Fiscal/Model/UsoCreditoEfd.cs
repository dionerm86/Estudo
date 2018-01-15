using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(UsoCreditoEfdDAO))]
    [PersistenceClass("uso_credito_efd")]
    public class UsoCreditoEfd
    {
        #region Propriedades

        [PersistenceProperty("IDUSOCREDITO", PersistenceParameterType.IdentityKey)]
        public uint IdUsoCredito { get; set; }

        [PersistenceProperty("IDCREDITO")]
        public uint IdCredito { get; set; }

        [PersistenceProperty("VALORUSADO")]
        public decimal ValorUsado { get; set; }

        [PersistenceProperty("PERIODOUSO")]
        public string PeriodoUso { get; set; }

        #endregion
    }
}