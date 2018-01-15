using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(UsoRetalhoProducaoDAO))]
    [PersistenceClass("uso_retalho_producao")]
    public class UsoRetalhoProducao
    {
        #region Propriedades

        [PersistenceProperty("IdUsoRetalhoProducao", PersistenceParameterType.IdentityKey)]
        public uint IdUsoRetalhoProducao { get; set; }

        [PersistenceProperty("IdRetalhoProducao")]
        public uint IdRetalhoProducao { get; set; }

        [PersistenceProperty("IdProdPedProducao")]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("VinculadoImpressao")]
        public bool VinculadoImpressao { get; set; }

        [PersistenceProperty("Cancelado")]
        public bool Cancelado { get; set; }

        #endregion
    }
}
