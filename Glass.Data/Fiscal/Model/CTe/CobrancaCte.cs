using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(CobrancaCteDAO))]
    [PersistenceClass("cobranca_cte")]
    public class CobrancaCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NUMEROFATURA")]
        public string NumeroFatura { get; set; }

        [PersistenceProperty("VALORORIGFATURA")]
        public decimal ValorOrigFatura { get; set; }

        [PersistenceProperty("DESCONTOFATURA")]
        public decimal DescontoFatura { get; set; }

        [PersistenceProperty("VALORLIQUIDOFATURA")]
        public decimal ValorLiquidoFatura { get; set; }

        [PersistenceProperty("GERARCONTASPAGAR")]
        public bool GerarContasPagar { get; set; }

        [PersistenceProperty("IDCONTA")]
        public uint? IdConta { get; set; }

        #endregion
    }
}
