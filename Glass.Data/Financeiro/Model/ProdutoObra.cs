using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoObraDAO))]
    [PersistenceClass("produto_obra")]
    public class ProdutoObra
    {
        #region Propriedades

        [PersistenceProperty("IDPRODOBRA", PersistenceParameterType.IdentityKey)]
        public uint IdProdObra { get; set; }

        [PersistenceProperty("IDOBRA")]
        public uint IdObra { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("VALORUNIT")]
        public decimal ValorUnitario { get; set; }

        [PersistenceProperty("TAMANHOMAX")]
        public float TamanhoMaximo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODINTERNO", DirectionParameter.InputOptional)]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRPRODUTO", DirectionParameter.InputOptional)]
        public string DescrProduto { get; set; }

        [PersistenceProperty("IDCLIENTE", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("TOTM2UTILIZADO", DirectionParameter.InputOptional)]
        public decimal TotM2Utilizado { get; set; }

        [PersistenceProperty("TOTALCALC", DirectionParameter.InputOptional)]
        public decimal TotalCalc { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal TotalProduto
        {
            get { return ValorUnitario * (decimal)TamanhoMaximo; }
        }

        #endregion
    }
}