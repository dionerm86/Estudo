using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CondutoresDAO))]
    [PersistenceClass("condutores")]
    public class Condutores : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONDUTOR", PersistenceParameterType.IdentityKey)]
        public int IdCondutor { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("CPFCNPJ")]
        public string CpfCnpj { get; set; }
    }
}
