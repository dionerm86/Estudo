using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncionarioSetorDAO))]
    [PersistenceClass("funcionario_setor")]
    public class FuncionarioSetor : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }

        [PersistenceProperty("IDSETOR", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Setor), "IdSetor")]
        public int IdSetor { get; set; }

        [PersistenceProperty("DESCRSETOR", DirectionParameter.InputOptional)]
        public string DescrSetor { get; set; }

        #endregion
    }
}