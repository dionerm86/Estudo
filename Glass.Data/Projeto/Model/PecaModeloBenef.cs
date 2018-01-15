using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PecaModeloBenefDAO))]
    [PersistenceClass("peca_modelo_benef")]
    public class PecaModeloBenef
    {
        #region Propriedades

        [PersistenceProperty("IDPECAMODELOBENEF", PersistenceParameterType.IdentityKey)]
        public uint IdPecaModeloBenef { get; set; }

        [PersistenceProperty("IDPECAPROJMOD")]
        public uint IdPecaProjMod { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        public uint IdBenefConfig { get; set; }

        [PersistenceProperty("QTD")]
        public int Qtd { get; set; }

        [PersistenceProperty("LAPLARG")]
        public int LapLarg { get; set; }

        [PersistenceProperty("LAPALT")]
        public int LapAlt { get; set; }

        [PersistenceProperty("BISLARG")]
        public int BisLarg { get; set; }

        [PersistenceProperty("BISALT")]
        public int BisAlt { get; set; }

        [PersistenceProperty("ESPFURO")]
        public int EspFuro { get; set; }

        [PersistenceProperty("ESPBISOTE")]
        public float EspBisote { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal Valor { get; set; }

        public decimal Custo { get; set; }

        #endregion
    }
}