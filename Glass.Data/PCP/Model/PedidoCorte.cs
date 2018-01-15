using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PedidoCorteDAO))]
	[PersistenceClass("pedido_corte")]
	public class PedidoCorte
    {
        #region Enumeradores

        public enum SituacaoEnum : int
        {
            Confirmado=1,
            Producao,
            Pronto,
            Entregue
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPEDIDOCORTE", PersistenceParameterType.IdentityKey)]
        public uint IdPedidoCorte { get; set; }

        [PersistenceProperty("IDPEDIDO")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("IDFUNCPRODUCAO")]
        public uint IdFuncProducao { get; set; }

        [PersistenceProperty("IDFUNCENTREGUE")]
        public uint? IdFuncEntregue { get; set; }

        [PersistenceProperty("DATAPRODUCAO")]
        public DateTime? DataProducao { get; set; }

        [PersistenceProperty("DATAPRONTO")]
        public DateTime? DataPronto { get; set; }

        [PersistenceProperty("DATAENTREGUE")]
        public DateTime? DataEntregue { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion
    }
}