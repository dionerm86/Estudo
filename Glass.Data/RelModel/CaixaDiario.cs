using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(CaixaDiarioDAO))]
    public class CaixaDiario
    {
        #region Propriedades

        [PersistenceProperty("DescrCxDiario")]
        public string DescrCaixaDiario { get; set; }

        [PersistenceProperty("Total")]
        public decimal Total { get; set; }

        #endregion
    }
}