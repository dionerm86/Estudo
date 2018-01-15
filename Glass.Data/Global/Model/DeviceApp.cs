using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("device_app")]
    public class DeviceApp : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdCliente", PersistenceParameterType.Key)]
        public int IdCliente { get; set; }

        [PersistenceProperty("Uuid", PersistenceParameterType.Key)]
        public string Uuid { get; set; }

        [PersistenceProperty("Token")]
        public string Token { get; set; }
    }
}
