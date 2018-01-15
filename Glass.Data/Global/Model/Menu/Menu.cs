using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MenuDAO))]
    [PersistenceClass("menu")]
    public class Menu : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDMENU", PersistenceParameterType.Key)]
        public int IdMenu { get; set; }

        [PersistenceProperty("IDMENUPAI")]
        [PersistenceForeignKey(typeof(Menu), "IdMenu")]
        public int? IdMenuPai { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("URL")]
        public string Url { get; set; }

        [PersistenceProperty("NUMSEQ")]
        public int NumSeq { get; set; }

        [PersistenceProperty("IDMODULO")]
        public Helper.Config.Modulo IdModulo { get; set; }

        [PersistenceProperty("OBSERVACAO")]
        public string Observacao { get; set; }

        [PersistenceProperty("EXIBIRLITE")]
        public bool ExibirLite { get; set; }
    }
}
