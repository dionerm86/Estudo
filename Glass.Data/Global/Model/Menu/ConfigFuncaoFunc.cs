using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("config_funcao_func")]
    public class ConfigFuncaoFunc : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONFIGFUNCAOFUNC", PersistenceParameterType.IdentityKey)]
        public int IdConfigFuncaoFunc { get; set; }

        [PersistenceProperty("IDFUNCAOMENU")]
        [PersistenceForeignKey(typeof(FuncaoMenu), "IdFuncaoMenu")]
        public int IdFuncaoMenu { get; set; }

        [PersistenceProperty("IDFUNC")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }
    }
}
