using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(InfoPedidosDAO))]
    [PersistenceClass("info_pedidos")]
    public class InfoPedidos
    {
        #region Propriedades

        [PersistenceProperty("Data", DirectionParameter.InputOptional)]
        public DateTime Data { get; set; }

        [PersistenceProperty("TotalFastDelivery")]
        public double TotalFastDelivery { get; set; }
        
        #endregion
    }
}