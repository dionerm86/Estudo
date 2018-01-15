using System;
using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoFornoDAO))]
    [PersistenceClass("producao_forno")]
    public class ProducaoForno
    {
        #region Propriedades

        [PersistenceProperty("Data")]
        public DateTime Data { get; set; }

        [PersistenceProperty("TotM2PedidoVenda")]
        public decimal TotM2PedidoVenda { get; set; }

        [PersistenceProperty("TotM2PedidoProducao")]
        public decimal TotM2PedidoProducao { get; set; }

        [PersistenceProperty("NomePrimSetor")]
        public string NomePrimSetor { get; set; }

        [PersistenceProperty("TotM2PrimSetor")]
        public decimal TotM2PrimSetor { get; set; }

        [PersistenceProperty("TotM2FornoProducao")]
        public decimal TotM2FornoProducao { get; set; }

        [PersistenceProperty("TotM2FornoPerda")]
        public decimal TotM2FornoPerda { get; set; }

        [PersistenceProperty("Obs")]
        public string Obs { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal TotM2Pedido
        {
            get { return TotM2PedidoVenda + TotM2PedidoProducao; }
        }

        public decimal TotM2FornoLiquido
        {
            get { return TotM2FornoProducao - TotM2FornoPerda; }
        }

        public decimal DiferencasForno
        {
            get { return TotM2FornoLiquido - TotM2Pedido; }
        }

        public decimal DiferencasPrimSetor
        {
            get { return TotM2FornoProducao - TotM2PrimSetor; }
        }

        #endregion
    }
}