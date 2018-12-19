using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(RelacaoBoxProducaoDAO))]
    [PersistenceClass("relacao_box")]
    public class RelacaoBoxProducao
    {
        #region Propriedades

        [PersistenceProperty("MODELO")]
        public string Modelo { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("DESCRCORVIDRO")]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("QTDEANTERIOR")]
        public long QtdeAnterior { get; set; }

        [PersistenceProperty("QTDE")]
        public long Qtde { get; set; }

        [PersistenceProperty("QTDEPRODUZIR")]
        public long QtdeProduzir { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public float TotM
        {
            get { return Glass.Global.CalculosFluxo.ArredondaM2(null, Largura, (int)Altura, 1F, 0, false, 0, true); }
        }

        #endregion
    }
}
