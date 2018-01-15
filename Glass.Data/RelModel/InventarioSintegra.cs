using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(InventarioSintegraDAO))]
    public class InventarioSintegra
    {
        #region Propriedades

        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [PersistenceProperty("QTD")]
        public double Qtd { get; set; }

        [PersistenceProperty("TOTAL")]
        public decimal Total { get; set; }

        #endregion
    }
}