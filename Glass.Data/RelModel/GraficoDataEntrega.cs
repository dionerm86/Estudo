using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(GraficoDataEntregaDAO))]
    public class GraficoDataEntrega
    {
        [PersistenceProperty("DATAENTREGA")]
        public string DataEntrega { get; set; }

        [PersistenceProperty("TOTALM2")]
        public decimal TotalM2 { get; set; }

        [PersistenceProperty("META")]
        public decimal Meta { get; set; }
    }
}
