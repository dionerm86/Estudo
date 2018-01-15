using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("config_funcao_tipo_func")]
    public class ConfigFuncaoTipoFunc : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONFIGFUNCAOTIPOFUNC", PersistenceParameterType.IdentityKey)]
        public int IdConfigFuncaoTipoFunc { get; set; }

        [PersistenceProperty("IDTIPOFUNC")]
        [PersistenceForeignKey(typeof(TipoFuncionario), "IdTipoFunc")]
        public int IdTipoFunc { get; set; }

        [PersistenceProperty("IDFUNCAOMENU")]
        [PersistenceForeignKey(typeof(FuncaoMenu), "IdFuncaoMenu")]
        public int IdFuncaoMenu { get; set; }
    }
}
