using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FeriadoDAO))]
    [PersistenceClass("feriado")]
    public class Feriado : ModelBaseCadastro
    {
        #region Propriedades

        [PersistenceProperty("IDFERIADO", PersistenceParameterType.IdentityKey)]
        public int IdFeriado { get; set; }

        [PersistenceProperty("Dia")]
        public int Dia { get; set; }

        [PersistenceProperty("Mes")]
        public int Mes { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }	
}