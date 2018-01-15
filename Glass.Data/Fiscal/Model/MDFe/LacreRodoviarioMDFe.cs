using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(LacreRodoviarioMDFeDAO))]
    [PersistenceClass("lacre_rodoviario_mdfe")]
    public class LacreRodoviarioMDFe
    {
        [PersistenceProperty("IDRODOVIARIO")]
        [PersistenceForeignKey(typeof(RodoviarioMDFe), "IdRodoviario")]
        public int IdRodoviario { get; set; }

        [PersistenceProperty("LACRE")]
        public string Lacre { get; set; }
    }
}
