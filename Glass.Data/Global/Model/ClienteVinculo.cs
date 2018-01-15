using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ClienteVinculoDAO))]
    [PersistenceClass("cliente_vinculo")]
    public class ClienteVinculo
    {
        #region Propriedades

        [PersistenceProperty("IDCLIENTE", PersistenceParameterType.Key)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("IDCLIENTEVINCULO", PersistenceParameterType.Key)]
        public uint IdClienteVinculo { get; set; }

        #endregion
    }
}