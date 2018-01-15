using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FlagArqMesaArqCalcEngineDAO))]
    [PersistenceClass("flag_arq_mesa_arq_calcengine")]
    public class FlagArqMesaArqCalcEngine
    {
        #region Propiedades

        [PersistenceProperty("IdArquivoCalcEngine", PersistenceParameterType.Key)]
        public int IdArquivoCalcEngine { get; set; }

        [PersistenceProperty("IdFlagArqMesa", PersistenceParameterType.Key)]
        public int IdFlagArqMesa { get; set; }

        #endregion
    }
}
