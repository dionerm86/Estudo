using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("config_menu")]
    public class ConfigMenu : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONFIGMENU", PersistenceParameterType.IdentityKey)]
        public int IdConfigMenu { get; set; }

        [PersistenceProperty("IDMENU")]
        [PersistenceForeignKey(typeof(Menu), "IdMenu")]
        public int IdMenu { get; set; }

        [PersistenceProperty("IDCONFIG")]
        [PersistenceForeignKey(typeof(Configuracao), "IdConfig")]
        public int IdConfig { get; set; }
    }
}
