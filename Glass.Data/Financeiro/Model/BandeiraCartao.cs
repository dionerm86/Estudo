using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(BandeiraCartaoDAO))]
    [PersistenceClass("bandeira_cartao")]
    public class BandeiraCartao : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDBANDEIRACARTAO", PersistenceParameterType.IdentityKey)]
        public uint IdBandeiraCartao { get; set; }

        [Log("Bandeira de Cartão")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool PodeExcluir
        {
            get { return !BandeiraCartaoDAO.Instance.BandeiraCartaoEmUso(IdBandeiraCartao); }
        }

        #endregion
    }
}
