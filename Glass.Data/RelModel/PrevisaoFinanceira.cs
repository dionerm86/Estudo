using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PrevisaoFinanceiraDAO))]
    [PersistenceClass("previsao_financeira")]
    public class PrevisaoFinanceira
    {
        #region Contas

        [PersistenceProperty("VencidasMais90Dias")]
        public decimal VencidasMais90Dias { get; set; }

        [PersistenceProperty("Vencidas90Dias")]
        public decimal Vencidas90Dias { get; set; }

        [PersistenceProperty("Vencidas60Dias")]
        public decimal Vencidas60Dias { get; set; }

        [PersistenceProperty("Vencidas30Dias")]
        public decimal Vencidas30Dias { get; set; }

        [PersistenceProperty("VencimentoHoje")]
        public decimal VencimentoHoje { get; set; }

        [PersistenceProperty("Vencer30Dias")]
        public decimal Vencer30Dias { get; set; }

        [PersistenceProperty("Vencer60Dias")]
        public decimal Vencer60Dias { get; set; }

        [PersistenceProperty("Vencer90Dias")]
        public decimal Vencer90Dias { get; set; }

        [PersistenceProperty("VencerMais90Dias")]
        public decimal VencerMais90Dias { get; set; }

        #endregion

        #region Cheques

        [PersistenceProperty("ChequesVencidosMais90Dias")]
        public decimal ChequesVencidosMais90Dias { get; set; }

        [PersistenceProperty("ChequesVencidos90Dias")]
        public decimal ChequesVencidos90Dias { get; set; }

        [PersistenceProperty("ChequesVencidos60Dias")]
        public decimal ChequesVencidos60Dias { get; set; }

        [PersistenceProperty("ChequesVencidos30Dias")]
        public decimal ChequesVencidos30Dias { get; set; }

        [PersistenceProperty("ChequesVencimentoHoje")]
        public decimal ChequesVencimentoHoje { get; set; }

        [PersistenceProperty("ChequesVencer30Dias")]
        public decimal ChequesVencer30Dias { get; set; }

        [PersistenceProperty("ChequesVencer60Dias")]
        public decimal ChequesVencer60Dias { get; set; }

        [PersistenceProperty("ChequesVencer90Dias")]
        public decimal ChequesVencer90Dias { get; set; }

        [PersistenceProperty("ChequesVencerMais90Dias")]
        public decimal ChequesVencerMais90Dias { get; set; }

        #endregion
 
        #region Pedidos

        [PersistenceProperty("PedidosVencidosMais90Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencidosMais90Dias { get; set; }

        [PersistenceProperty("PedidosVencidos90Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencidos90Dias { get; set; }

        [PersistenceProperty("PedidosVencidos60Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencidos60Dias { get; set; }

        [PersistenceProperty("PedidosVencidos30Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencidos30Dias { get; set; }

        [PersistenceProperty("PedidosVencimentoHoje", DirectionParameter.InputOptional)]
        public decimal PedidosVencimentoHoje { get; set; }

        [PersistenceProperty("PedidosVencer30Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencer30Dias { get; set; }

        [PersistenceProperty("PedidosVencer60Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencer60Dias { get; set; }

        [PersistenceProperty("PedidosVencer90Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencer90Dias { get; set; }

        [PersistenceProperty("PedidosVencerMais90Dias", DirectionParameter.InputOptional)]
        public decimal PedidosVencerMais90Dias { get; set; }

        #endregion

        #region PrevisaoCustoFixo

        [PersistenceProperty("PrevisaoCustoFixoVencer30Dias", DirectionParameter.InputOptional)]
        public decimal? PrevisaoCustoFixoVencer30Dias { get; set; }

        [PersistenceProperty("PrevisaoCustoFixoVencer60Dias", DirectionParameter.InputOptional)]
        public decimal? PrevisaoCustoFixoVencer60Dias { get; set; }

        [PersistenceProperty("PrevisaoCustoFixoVencer90Dias", DirectionParameter.InputOptional)]
        public decimal? PrevisaoCustoFixoVencer90Dias { get; set; }

        #endregion

        #region Total

        public decimal TotalVencidasMais90Dias
        {
            get { return VencidasMais90Dias + ChequesVencidosMais90Dias + PedidosVencidosMais90Dias; }
        }

        public decimal TotalVencidas90Dias
        {
            get { return Vencidas90Dias + ChequesVencidos90Dias + PedidosVencidos90Dias; }
        }

        public decimal TotalVencidas60Dias
        {
            get { return Vencidas60Dias + ChequesVencidos60Dias + PedidosVencidos60Dias; }
        }

        public decimal TotalVencidas30Dias
        {
            get { return Vencidas30Dias + ChequesVencidos30Dias + PedidosVencidos30Dias; }
        }

        public decimal TotalVencimentoHoje
        {
            get { return VencimentoHoje + ChequesVencimentoHoje + PedidosVencimentoHoje; }
        }

        public decimal TotalVencer30Dias
        {
            get
            {
                return Vencer30Dias + ChequesVencer30Dias + PedidosVencer30Dias
                    + (PrevisaoCustoFixoVencer30Dias.HasValue ? PrevisaoCustoFixoVencer30Dias.Value : 0);
            }
        }

        public decimal TotalVencer60Dias
        {
            get 
            {
                return Vencer60Dias + ChequesVencer60Dias + PedidosVencer60Dias
                    + (PrevisaoCustoFixoVencer60Dias.HasValue ? PrevisaoCustoFixoVencer60Dias.Value : 0); 
            }
        }

        public decimal TotalVencer90Dias
        {
            get
            {
                return Vencer90Dias + ChequesVencer90Dias + PedidosVencer90Dias
                    + (PrevisaoCustoFixoVencer90Dias.HasValue ? PrevisaoCustoFixoVencer90Dias.Value : 0);
            }
        }

        public decimal TotalVencerMais90Dias
        {
            get { return VencerMais90Dias + ChequesVencerMais90Dias + PedidosVencerMais90Dias; }
        }

        #endregion
    }
}