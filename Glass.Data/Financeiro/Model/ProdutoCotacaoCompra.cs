using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoCotacaoCompraDAO))]
    [PersistenceClass("produto_cotacao_compra")]
    public class ProdutoCotacaoCompra
    {
        #region Propriedades

        [PersistenceProperty("IDPRODCOTACAOCOMPRA", PersistenceParameterType.IdentityKey)]
        public uint IdProdCotacaoCompra { get; set; }

        [PersistenceProperty("IDCOTACAOCOMPRA")]
        public uint IdCotacaoCompra { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("ALTURA")]
        public float Altura { get; set; }

        [PersistenceProperty("LARGURA")]
        public int Largura { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("TOTM")]
        public float TotM { get; set; }

        #endregion
    }
}
