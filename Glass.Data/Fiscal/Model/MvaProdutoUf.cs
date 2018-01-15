using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MvaProdutoUfDAO))]
    [PersistenceClass("mva_produto_uf")]
    public class MvaProdutoUf : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPROD", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int IdProd { get; set; }

        [PersistenceProperty("UFORIGEM", PersistenceParameterType.Key)]
        public string UfOrigem { get; set; }

        [PersistenceProperty("UFDESTINO", PersistenceParameterType.Key)]
        public string UfDestino { get; set; }

        [PersistenceProperty("MVAORIGINAL")]
        public float MvaOriginal { get; set; }

        [PersistenceProperty("MVASIMPLES")]
        public float MvaSimples { get; set; }

        #endregion
    }
}
