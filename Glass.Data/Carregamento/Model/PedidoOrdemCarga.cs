using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoOrdemCargaDAO))]
    [PersistenceClass("pedido_ordem_carga")]
    public class PedidoOrdemCarga
    {
        #region Propiedades

        [Log("Pedido")]
        [PersistenceProperty("IDPEDIDO", PersistenceParameterType.Key)]
        public uint IdPedido { get; set; }

        [Log("Ordem de Carga")]
        [PersistenceProperty("IDORDEMCARGA", PersistenceParameterType.Key)]
        public uint IdOrdemCarga { get; set; }

        #endregion
    }
}
