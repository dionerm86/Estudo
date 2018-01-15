using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoEntradaEstoqueDAO))]
    [PersistenceClass("produto_entrada_estoque")]
    public class ProdutoEntradaEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDPRODENTRADAESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdProdEntradaEstoque { get; set; }

        [PersistenceProperty("IDENTRADAESTOQUE")]
        public uint IdEntradaEstoque { get; set; }

        [PersistenceProperty("IDPRODCOMPRA")]
        public uint? IdProdCompra { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("QTDE")]
        public float QtdeEntrada { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPROD", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        #endregion
    }
}