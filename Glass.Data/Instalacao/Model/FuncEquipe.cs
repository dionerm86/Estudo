using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncEquipeDAO))]
	[PersistenceClass("func_equipe")]
	public class FuncEquipe
    {
        #region Propriedades

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        public uint Idfunc { get; set; }

        [PersistenceProperty("IDEQUIPE", PersistenceParameterType.Key)]
        public uint IdEquipe { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("TIPOFUNC", DirectionParameter.InputOptional)]
        public string TipoFunc { get; set; }

        #endregion
    }
}