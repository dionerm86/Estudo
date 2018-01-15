using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoNfItemProjetoDAO))]
    [PersistenceClass("produto_nf_item_projeto")]
    public class ProdutoNfItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDPRODNF")]
        public uint IdProdNf { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint IdItemProjeto { get; set; }

        #endregion
    }
}
