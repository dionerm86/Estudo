using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("carga_tributaria_media_mt"),
    PersistenceBaseDAO(typeof(DAL.CargaTributariaMediaMTDAO))]
    public class CargaTributariaMediaMT
    {
        [PersistenceProperty("CNAE", PersistenceParameterType.Key)]
        public string Cnae { get; set; }

        [PersistenceProperty("CARGATRIBUTARIAMEDIA")]
        public float CargaTributariaMedia { get; set; }

        [PersistenceProperty("CARGATRIBUTARIAFUNDO")]
        public float CargaTributariaFundo { get; set; }
    }
}
