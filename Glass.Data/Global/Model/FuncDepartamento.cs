using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncDepartamentoDAO))]
    [PersistenceClass("func_departamento")]
    public class FuncDepartamento : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDDEPARTAMENTO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Departamento), "IdDepartamento")]
        public int IdDepartamento { get; set; }

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEDEPARTAMENTO", DirectionParameter.InputOptional)]
        public string NomeDepartamento { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        #endregion
    }
}