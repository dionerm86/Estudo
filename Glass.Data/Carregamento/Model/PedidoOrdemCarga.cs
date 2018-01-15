using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoOrdemCargaDAO))]
    [PersistenceClass("pedido_ordem_carga")]
    public class PedidoOrdemCarga
    {
        #region Contrutores

        public PedidoOrdemCarga()
        {

        }

        public PedidoOrdemCarga(uint idPedido, uint idOrdemCarga)
        {
            IdPedido = idPedido;
            IdOrdemCarga = idOrdemCarga;
        }

        #endregion

        #region Propiedades

        [PersistenceProperty("IdPedidoOrdemCarga", PersistenceParameterType.IdentityKey)]
        public int IdPedidoOrdemCarga { get; set; }

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO")]
        [PersistenceForeignKey(typeof(Pedido), "IdPedido")]
        public uint IdPedido { get; set; }

        [Log("Ordem de Carga")]
        [PersistenceProperty("IDORDEMCARGA")]
        [PersistenceForeignKey(typeof(OrdemCarga), "IdOrdemCarga")]
        public uint IdOrdemCarga { get; set; }

        #endregion
    }
}
