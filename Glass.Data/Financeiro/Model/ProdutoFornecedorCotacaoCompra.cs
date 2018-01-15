using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoFornecedorCotacaoCompraDAO))]
    [PersistenceClass("produto_fornecedor_cotacao_compra")]
    public class ProdutoFornecedorCotacaoCompra
    {
        #region Propriedades

        [PersistenceProperty("IDCOTACAOCOMPRA", PersistenceParameterType.Key)]
        public uint IdCotacaoCompra { get; set; }

        [PersistenceProperty("IDPROD", PersistenceParameterType.Key)]
        public uint IdProd { get; set; }

        [PersistenceProperty("IDFORNEC", PersistenceParameterType.Key)]
        public uint IdFornec { get; set; }

        [PersistenceProperty("IDPARCELA")]
        public ulong? IdParcela { get; set; }

        [PersistenceProperty("CUSTOUNIT")]
        public decimal CustoUnit { get; set; }

        [PersistenceProperty("PRAZOENTREGADIAS")]
        public long PrazoEntregaDias { get; set; }

        [PersistenceProperty("DATASPAGAMENTOS")]
        public string DatasPagamentos { get; set; }

        #endregion

        #region Propriedades estendidas

        [PersistenceProperty("CADASTRADO", DirectionParameter.InputOptional)]
        public bool Cadastrado { get; set; }

        [PersistenceProperty("CUSTOTOTAL", DirectionParameter.InputOptional)]
        public decimal CustoTotal { get; set; }

        #endregion
    }
}
