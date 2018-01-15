using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(BenefConfigAssocDAO))]
    [PersistenceClass("benef_config_assoc")]
    public class BenefConfigAssoc
    {
        #region Propriedades

        [PersistenceProperty("IDBENEFCONFIG", PersistenceParameterType.Key)]
        public uint IdBenefConfig { get; set; }

        [PersistenceProperty("IDBENEFCONFIGASSOC", PersistenceParameterType.Key)]
        public uint IdBenefConfigAssoc { get; set; }

        [PersistenceProperty("COBRARASSOC")]
        public bool CobrarAssoc { get; set; }

        [PersistenceProperty("BLOQUEARASSOC")]
        public bool BloquearAssoc { get; set; }

        [PersistenceProperty("ALTURABENEF")]
        public int AlturaBenef { get; set; }

        [PersistenceProperty("LARGURABENEF")]
        public int LarguraBenef { get; set; }

        [PersistenceProperty("ESPESSURABENEF")]
        public float EspessuraBenef { get; set; }

        [PersistenceProperty("QTDEBENEF")]
        public int QtdeBenef { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("IDPARENTASSOC", DirectionParameter.InputOptional)]
        public uint? IdParentAssoc { get; set; }

        [PersistenceProperty("TIPOCONTROLEASSOC", DirectionParameter.InputOptional)]
        public int TipoControleAssoc { get; set; }

        #endregion
    }
}
