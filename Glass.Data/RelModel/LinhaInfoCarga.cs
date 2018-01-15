using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LinhaInfoCargaDAO))]
    public class LinhaInfCarga
    {
        public string TipoMedida1 { get; set; }
        public string Quantidade1 { get; set; }
        public string UnidadeMedida1 { get; set; }
        public string TipoMedida2 { get; set; }
        public string Quantidade2 { get; set; }
        public string UnidadeMedida2 { get; set; }
        public string TipoMedida3 { get; set; }
        public string Quantidade3 { get; set; }
        public string UnidadeMedida3 { get; set; }
    }
}
