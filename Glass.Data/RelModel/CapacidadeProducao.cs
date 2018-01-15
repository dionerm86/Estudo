using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(CapacidadeProducaoDAO)),
    PersistenceClass("capacidade_producao")]
    public class CapacidadeProducao
    {
        [PersistenceProperty("IDSETOR")]
        public uint IdSetor { get; set; }

        [PersistenceProperty("TOTM")]
        public decimal TotM { get; set; }

        #region Propriedades estendidas

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        internal uint? IdPedido { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        internal string Criterio { get; set; }

        #endregion
    }
}
