using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceClass("roteiro_producao_setor")]
    [PersistenceBaseDAO(typeof(RoteiroProducaoSetorDAO))]
    public class RoteiroProducaoSetor : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDROTEIROPRODUCAO", PersistenceParameterType.Key)]
        public int IdRoteiroProducao { get; set; }

        [PersistenceProperty("IDSETOR", PersistenceParameterType.Key)]
        public uint IdSetor { get; set; }

        #endregion
    }
}
