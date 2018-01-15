using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidosCompraDAO))]
    [PersistenceClass("pedidos_compra")]
    public class PedidosCompra
    {
        #region Propriedades

        [PersistenceProperty("IDCOMPRA", PersistenceParameterType.IdentityKey)]
        public uint IdCompra { get; set; }

        [PersistenceProperty("IDPEDIDO", PersistenceParameterType.IdentityKey)]
        public uint IdPedido { get; set; }

        [PersistenceProperty("PRODUTOBENEF")]
        public bool? ProdutoBenef { get; set; }

        #endregion
    }
}