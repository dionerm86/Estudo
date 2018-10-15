using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CondutoresDAO))]
    [PersistenceClass("condutores")]
    public class Condutores : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador do condutor.
        /// </summary>
        [PersistenceProperty("IDCONDUTOR", PersistenceParameterType.IdentityKey)]
        public int IdCondutor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do condutor.
        /// </summary>
        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define O CPF do Condutor.
        /// </summary>
        [PersistenceProperty("CPF")]
        public string Cpf { get; set; }
    }
}
