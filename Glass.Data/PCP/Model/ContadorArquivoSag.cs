using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ContadorArquivoSagDAO))]
    [PersistenceClass("contador_arquivo_sag")]
    public class ContadorArquivoSag
    {
        #region Propriedades

        [PersistenceProperty("CONTADOR")]
        public uint Contador { get; set; }

        #endregion
    }
}