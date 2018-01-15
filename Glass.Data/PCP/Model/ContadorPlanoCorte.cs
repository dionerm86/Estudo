using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContadorPlanoCorteDAO))]
    [PersistenceClass("contador_plano_corte")]
    public class ContadorPlanoCorte
    {
        #region Propriedades

        [PersistenceProperty("CONTADOR")]
        public uint Contador { get; set; }

        #endregion
    }
}