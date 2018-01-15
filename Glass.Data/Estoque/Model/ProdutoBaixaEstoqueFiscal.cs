using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoBaixaEstoqueFiscalDAO))]
    [PersistenceClass("produto_baixa_estoque_fiscal")]
    public class ProdutoBaixaEstoqueFiscal : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPRODBAIXAESTFISC", PersistenceParameterType.IdentityKey)]
        public int IdProdBaixaEstFisc { get; set; }

        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Principal")]
        public int IdProd { get; set; }

        [PersistenceProperty("IDPRODBAIXA")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Baixa")]
        public int IdProdBaixa { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        #endregion
    }
}