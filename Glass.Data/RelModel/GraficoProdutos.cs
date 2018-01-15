using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(GraficoProdutosDAO))]
    public class GraficoProdutos
    {
        #region Propriedades

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("TOTALVENDA")]
        public decimal TotalVenda { get; set; }

        [PersistenceProperty("TOTALQTDE")]
        public double TotalQtde { get; set; }

        [PersistenceProperty("DESCRPRODUTO")]
        public string DescrProduto { get; set; }

        [PersistenceProperty("DATAVENDA")]
        public string DataVenda { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        [PersistenceProperty("TIPO")]
        public long TipoValor { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal ValorExibir
        {
            get { return TipoValor == 1 ? (decimal)TotalQtde : TotalVenda; }
        }

        #endregion
    }

    public class GraficoProdutosImagem
    {
        public byte[] Buffer { get; set; }
    }
}