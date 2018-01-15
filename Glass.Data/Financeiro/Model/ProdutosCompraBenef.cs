using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutosCompraBenefDAO))]
    [PersistenceClass("produtos_compra_benef")]
    public class ProdutosCompraBenef
    {
        #region Propriedades

        [PersistenceProperty("IDPRODCOMPRABENEF", PersistenceParameterType.IdentityKey)]
        public uint IdProdCompraBenef { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        public uint IdBenefConfig { get; set; }

        [PersistenceProperty("IDPRODCOMPRA")]
        public uint IdProdCompra { get; set; }

        [PersistenceProperty("QTD")]
        public int Qtde { get; set; }

        private decimal _valorUnit;

        [PersistenceProperty("VALORUNIT")]
        public decimal ValorUnit
        {
            get { return Qtde == 0 ? _valorUnit : GenericBenef.GetValorUnit(_valorUnit, Valor, Qtde); }
            set { _valorUnit = value; }
        }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("LAPLARG")]
        public int LapLarg { get; set; }

        [PersistenceProperty("LAPALT")]
        public int LapAlt { get; set; }

        [PersistenceProperty("BISLARG")]
        public int BisLarg { get; set; }

        [PersistenceProperty("BISALT")]
        public int BisAlt { get; set; }

        [PersistenceProperty("ESPFURO")]
        public int EspFuro { get; set; }

        [PersistenceProperty("ESPBISOTE")]
        public float EspBisote { get; set; }

        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRBENEF", DirectionParameter.InputOptional)]
        public string DescrBenef { get; set; }

        [PersistenceProperty("ValorTabela", DirectionParameter.InputOptional)]
        public decimal ValorTabela { get; set; }

        #endregion
    }
}