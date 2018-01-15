using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoCfopDAO))]
    [PersistenceClass("tipo_cfop")]
    public class TipoCfop : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOCFOP", PersistenceParameterType.IdentityKey)]
        public int IdTipoCfop { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("INDUSTRIALIZACAO")]
        public bool Industrializacao { get; set; }

        [PersistenceProperty("DEVOLUCAO")]
        public bool Devolucao { get; set; }

        #endregion
    }
}