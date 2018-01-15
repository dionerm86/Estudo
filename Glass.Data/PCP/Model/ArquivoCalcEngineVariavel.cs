using GDA;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(Glass.Data.DAL.ArquivoCalcEngineVariavelDAO))]
    [PersistenceClass("arquivo_calcengine_variavel")]
    public class ArquivoCalcEngineVariavel
    {
        #region Propriedades

        [PersistenceProperty("IDARQUIVOCALCENGINEVAR", PersistenceParameterType.IdentityKey)]
        public uint IdArquivoCalcEngineVar { get; set; }

        [PersistenceProperty("IDARQUIVOCALCENGINE")]
        public uint IdArquivoCalcEngine { get; set; }

        [PersistenceProperty("VARIAVELCALCENGINE")]
        public string VariavelCalcEngine { get; set; }

        [PersistenceProperty("VARIAVELSISTEMA")]
        public string VariavelSistema { get; set; }

        [PersistenceProperty("VALORPADRAO")]
        public decimal ValorPadrao { get; set; }

        #endregion
    }
}