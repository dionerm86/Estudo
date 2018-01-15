using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DepartamentoDAO))]
    [PersistenceClass("departamento")]
    public class Departamento : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDDEPARTAMENTO", PersistenceParameterType.IdentityKey)]
        public int IdDepartamento { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}