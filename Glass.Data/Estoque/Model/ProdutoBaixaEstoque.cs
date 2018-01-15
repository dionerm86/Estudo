using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoBaixaEstoqueDAO))]
    [PersistenceClass("produto_baixa_estoque")]
    public class ProdutoBaixaEstoque : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPRODBAIXAEST", PersistenceParameterType.IdentityKey)]
        public int IdProdBaixaEst { get; set; }

        [PersistenceProperty("IDPROD")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Principal")]
        public int IdProd { get; set; }

        [PersistenceProperty("IDPRODBAIXA")]
        [PersistenceForeignKey(typeof(Produto), "IdProd", "Baixa")]
        public int IdProdBaixa { get; set; }

        [PersistenceProperty("QTDE")]
        public float Qtde { get; set; }

        [PersistenceProperty("IdProcesso")]
        [PersistenceForeignKey(typeof(EtiquetaProcesso), "IdAplicacao")]
        public int IdProcesso { get; set; }

        [PersistenceProperty("IdAplicacao")]
        [PersistenceForeignKey(typeof(EtiquetaAplicacao), "IdAplicacao")]
        public int IdAplicacao { get; set; }

        [PersistenceProperty("Altura")]
        public int Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("Forma")]
        public string Forma { get; set; }

        #endregion
    }
}