using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidosNotaFiscalDAO))]
    [PersistenceClass("pedidos_nota_fiscal")]
    public class PedidosNotaFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDPEDIDONF", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoNf { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO")]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("IDCARREGAMENTO")]
        public uint? IdCarregamento { get; set; }

        #endregion
    }
}