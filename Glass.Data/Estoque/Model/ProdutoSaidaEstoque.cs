using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoSaidaEstoqueDAO))]
    [PersistenceClass("produto_saida_estoque")]
    public class ProdutoSaidaEstoque
    {
        #region Propriedades

        [PersistenceProperty("IDPRODSAIDAESTOQUE", PersistenceParameterType.IdentityKey)]
        public uint IdProdSaidaEstoque { get; set; }

        [PersistenceProperty("IDSAIDAESTOQUE")]
        public uint IdSaidaEstoque { get; set; }

        [PersistenceProperty("IDPRODPED")]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("QTDE")]
        public float QtdeSaida { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPROD", DirectionParameter.InputOptional)]
        public string DescrProd { get; set; }

        #endregion
    }
}