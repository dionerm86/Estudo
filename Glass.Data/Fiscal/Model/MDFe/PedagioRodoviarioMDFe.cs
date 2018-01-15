using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(PedagioRodoviarioMDFeDAO))]
    [PersistenceClass("pedagio_rodoviario_mdfe")]
    public class PedagioRodoviarioMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDRODOVIARIO")]
        [PersistenceForeignKey(typeof(RodoviarioMDFe), "IdRodoviario")]
        public int IdRodoviario { get; set; }
        
        [PersistenceProperty("IDFORNECEDOR")]
        [PersistenceForeignKey(typeof(Fornecedor), "IdFornec")]
        public int IdFornecedor { get; set; }

        [PersistenceProperty("RESPONSAVELPEDAGIO")]
        public ResponsavelEnum ResponsavelPedagio { get; set; }

        [PersistenceProperty("NUMEROCOMPRA")]
        public string NumeroCompra { get; set; }

        [PersistenceProperty("VALORVALEPEDAGIO")]
        public decimal ValorValePedagio { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNECEDOR", DirectionParameter.InputOptional)]
        public string NomeFornecedor { get; set; }

        #endregion
    }
}
