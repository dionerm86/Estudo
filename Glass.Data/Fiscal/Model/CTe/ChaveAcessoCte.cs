using GDA;

namespace Glass.Data.Model.Cte
{
    [PersistenceClass("chave_acesso_cte")]
    public class ChaveAcessoCte : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdChaveAcessoCte", PersistenceParameterType.IdentityKey)]
        public int IdChaveAcessoCte { get; set; }

        [PersistenceProperty("IdCte")]
        public int IdCte { get; set; }

        [PersistenceProperty("ChaveAcesso")]
        public string ChaveAcesso { get; set; }

        [PersistenceProperty("PIN")]
        public string PIN { get; set; }
    }
}
