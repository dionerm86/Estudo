using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(VeiculoRodoviarioMDFeDAO))]
    [PersistenceClass("veiculo_rodoviario_mdfe")]
    public class VeiculoRodoviarioMDFe
    {
        [PersistenceProperty("IDRODOVIARIO")]
        [PersistenceForeignKey(typeof(RodoviarioMDFe), "IdRodoviario")]
        public int IdRodoviario { get; set; }

        [PersistenceProperty("PLACA")]
        [PersistenceForeignKey(typeof(Veiculo), "Placa")]
        public string Placa { get; set; }
    }
}
