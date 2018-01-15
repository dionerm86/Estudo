using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoFornecedorDAO))]
    [PersistenceClass("produto_fornecedor")]
    public class ProdutoFornecedor : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPRODFORNEC", PersistenceParameterType.IdentityKey)]
        public int IdProdFornec { get; set; }

        [Log("Produto", "Descricao", typeof(ProdutoDAO))]
        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int IdProd { get; set; }

        [Log("Fornecedor", "Nome", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        [PersistenceForeignKey(typeof(Fornecedor), "IdFornec")]
        public int IdFornec { get; set; }

        [Log("Data de Vigência")]
        [PersistenceProperty("DATAVIGENCIA")]
        public DateTime? DataVigencia { get; set; }

        [Log("Cód. Produto Fornecedor")]
        [PersistenceProperty("CODFORNEC")]
        public string CodFornec { get; set; }

        [Log("Custo Compra")]
        [PersistenceProperty("CUSTOCOMPRA")]
        public decimal CustoCompra { get; set; }

        [Log("Prazo Entrega (Dias)")]
        [PersistenceProperty("PRAZOENTREGADIAS")]
        public int PrazoEntregaDias { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion
    }
}