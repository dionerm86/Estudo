using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PrecoAnteriorDAO))]
    [PersistenceClass("preco_anterior")]
    public class PrecoAnterior
    {
        #region Propriedades

        [PersistenceProperty("IDPROD", PersistenceParameterType.Key)]
        public uint IdProd { get; set; }

        [PersistenceProperty("CUSTOFABBASE")]
        public decimal Custofabbase { get; set; }

        [PersistenceProperty("CUSTOCOMPRA")]
        public decimal CustoCompra { get; set; }

        [PersistenceProperty("VALORATACADO")]
        public decimal ValorAtacado { get; set; }

        [PersistenceProperty("VALORBALCAO")]
        public decimal ValorBalcao { get; set; }

        [PersistenceProperty("VALOROBRA")]
        public decimal ValorObra { get; set; }

        [PersistenceProperty("VALORREPOSICAO")]
        public decimal ValorReposicao { get; set; }

        [PersistenceProperty("VALORFISCAL")]
        public decimal ValorFiscal { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        #endregion
    }
}