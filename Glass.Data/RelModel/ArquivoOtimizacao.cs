using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ArquivoOtimizacaoDAO))]
    [PersistenceClass("arquivo_otimizacao")]
    public class ArquivoOtimizacao
    {
        #region Propriedades

        [PersistenceProperty("Etiquetas")]
        public string Etiquetas { get; set; }

        [PersistenceProperty("Espessura")]
        public float Espessura { get; set; }

        [PersistenceProperty("CorVidro")]
        public string CorVidro { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrEspessura
        {
            get { return Espessura > 0 ? Espessura + "mm" : ""; }
        }

        public string Material
        {
            get { return CorVidro + (Espessura > 0 ? " / " + DescrEspessura : ""); }
        }

        #endregion
    }
}