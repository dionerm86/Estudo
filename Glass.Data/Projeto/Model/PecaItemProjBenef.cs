using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PecaItemProjBenefDAO))]
    [PersistenceClass("peca_item_proj_benef")]
    public class PecaItemProjBenef
    {
        #region Propriedades

        [PersistenceProperty("IDPECAITEMPROJBENEF", PersistenceParameterType.IdentityKey)]
        public uint IdPecaItemProjBenef { get; set; }

        [PersistenceProperty("IDPECAITEMPROJ")]
        public uint IdPecaItemProj { get; set; }

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
    }
}