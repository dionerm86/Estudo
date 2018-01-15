using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LinhaDocumentosOriginariosDAO))]
    public class LinhaDocumentosOriginarios
    {
        public string TipoDoc1 { get; set; }
        public string DocEmitenteNf1 { get; set; }
        public string Serie1 { get; set; }
        public string NumeroDoc1 { get; set; }
        public string TipoDoc2 { get; set; }
        public string DocEmitenteNf2 { get; set; }
        public string Serie2 { get; set; }
        public string NumeroDoc2 { get; set; }
    }
}
