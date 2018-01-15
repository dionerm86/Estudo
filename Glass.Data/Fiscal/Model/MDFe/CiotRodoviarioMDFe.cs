using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(CiotRodoviarioMDFeDAO))]
    [PersistenceClass("ciot_rodoviario_mdfe")]
    public class CiotRodoviarioMDFe
    {
        [PersistenceProperty("IDRODOVIARIO")]
        [PersistenceForeignKey(typeof(RodoviarioMDFe), "IdRodoviario")]
        public int IdRodoviario { get; set; }

        [PersistenceProperty("CIOT")]
        public string CIOT { get; set; }

        [PersistenceProperty("CPFCNPJCIOT")]
        public string CPFCNPJCIOT { get; set; }
    }
}
