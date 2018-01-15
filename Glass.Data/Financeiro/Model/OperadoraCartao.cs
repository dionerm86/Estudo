using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(OperadoraCartaoDAO))]
    [PersistenceClass("operadora_cartao")]
    public class OperadoraCartao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDOPERADORACARTAO", PersistenceParameterType.IdentityKey)]
        public uint IdOperadoraCartao { get; set; }

        [Log("Operadora de Cartão")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool PodeExcluir
        {
            get { return !OperadoraCartaoDAO.Instance.OperadoraCartaoEmUso(IdOperadoraCartao); }
        }

        #endregion
    }
}