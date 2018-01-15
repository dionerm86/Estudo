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

        [PersistenceProperty("TotalProducao")]
        public double TotalProducao { get; set; }

        [PersistenceProperty("TotalProducaoInterna")]
        public double TotalProducaoInterna { get; set; }

        #endregion

        #region Propriedades de Suporte

        public double TotalProducaoSemFastDelivery
        {
            get { return Math.Max(TotalProducao - TotalFastDelivery, 0); }
        }

        public double TotalProducaoGeral
        {
            get { return TotalProducaoSemFastDelivery + TotalProducaoInterna; }
        }

        #endregion
    }
}