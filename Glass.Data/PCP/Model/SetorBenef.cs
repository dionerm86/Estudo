using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SetorBenefDAO))]
    [PersistenceClass("setor_benef")]
    public class SetorBenef :  Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSETORBENEF", PersistenceParameterType.IdentityKey)]
        public int IdSetorBenef { get; set; }

        [PersistenceProperty("IDSETOR")]
        [PersistenceForeignKey(typeof(Setor), "IdSetor")]
        public int IdSetor { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        public int IdBenefConfig { get; set; }

        [PersistenceProperty("DESCRBENEF", DirectionParameter.InputOptional)]
        public string DescrBenef { get; set; }

        #endregion
    }
}