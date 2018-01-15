using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PontoFotoMedicaoDAO))]
    [PersistenceClass("ponto_foto_medicao")]
    public class PontoFotoMedicao
    {
        #region Propriedades

        [PersistenceProperty("IDPONTOFOTO", PersistenceParameterType.IdentityKey)]
        public uint IdPontoFoto { get; set; }

        [PersistenceProperty("IDFOTO")]
        public uint IdFoto { get; set; }

        [PersistenceProperty("COORDX")]
        public int CoordX { get; set; }

        [PersistenceProperty("COORDY")]
        public int CoordY { get; set; }

        #endregion
    }
}