using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FuncaoMenuDAO))]
    [PersistenceClass("funcao_menu")]
    public class FuncaoMenu : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDFUNCAOMENU", PersistenceParameterType.Key)]
        public int IdFuncaoMenu { get; set; }

        [PersistenceProperty("IDMENU")]
        [PersistenceForeignKey(typeof(Menu), "IdMenu")]
        public int IdMenu { get; set; }

        [PersistenceProperty("IDFUNCAO")]
        public int IdFuncao { get; set; }

        [PersistenceProperty("IDMODULO")]
        public Helper.Config.Modulo IdModulo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }
    }
}
