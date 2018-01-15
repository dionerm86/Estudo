using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("margem_lucro_cnae_mt"),
    PersistenceBaseDAO(typeof(DAL.MargemLucroCnaeMTDAO))]
    public class MargemLucroCnaeMT
    {
        [PersistenceProperty("CNAE", PersistenceParameterType.Key)]
        public string Cnae { get; set; }

        [PersistenceProperty("MARGEMLUCRO")]
        public float MargemLucro { get; set; }
    }
}
