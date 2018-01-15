using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(RoteiroProducaoEtiquetaDAO))]
    [PersistenceClass("roteiro_producao_etiqueta")]
    public class RoteiroProducaoEtiqueta
    {
        [PersistenceProperty("IDPRODPEDPRODUCAO", PersistenceParameterType.Key)]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("IDSETOR", PersistenceParameterType.Key)]
        public uint IdSetor { get; set; }

        [PersistenceProperty("ULTIMOSETOR")]
        public bool UltimoSetor { get; set; }
    }
}
