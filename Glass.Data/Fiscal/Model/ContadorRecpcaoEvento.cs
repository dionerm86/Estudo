using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContadorRecepcaoEventoDAO))]
    [PersistenceClass("Contador_Recepcao_Evento")]
    public class ContadorRecepcaoEvento
    {
        #region Propriedades

        [PersistenceProperty("CONTADOR")]
        public uint Contador { get; set; }

        #endregion
    }
}