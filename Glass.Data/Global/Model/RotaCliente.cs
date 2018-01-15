using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(RotaClienteDAO))]
    [PersistenceClass("rota_cliente")]
    public class RotaCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDROTACLIENTE", PersistenceParameterType.IdentityKey)]
        public int IdRotaCliente { get; set; }

        [PersistenceProperty("IDROTA")]
        [PersistenceForeignKey(typeof(Rota), "IdRota")]
        public int IdRota { get; set; }

        [PersistenceProperty("IDCLIENTE")]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int IdCliente { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        #endregion
    }
}