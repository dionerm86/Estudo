using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoTrocadoBenefDAO))]
    [PersistenceClass("produto_trocado_benef")]
    public class ProdutoTrocadoBenef
    {
        #region Propriedades

        [PersistenceProperty("IDPRODTROCADOBENEF", PersistenceParameterType.IdentityKey)]
        public uint IdProdTrocadoBenef { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        public uint IdBenefConfig { get; set; }

        [PersistenceProperty("IDPRODTROCADO")]
        public uint IdProdTrocado { get; set; }

        [PersistenceProperty("QTD")]
        public int Qtd { get; set; }

        private decimal _valorUnit;

        [PersistenceProperty("VALORUNIT")]
        public decimal ValorUnit
        {
            get { return GenericBenef.GetValorUnit(_valorUnit, Valor, Qtd); }
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

        [PersistenceProperty("ESPBISOTE")]
        public float EspBisote { get; set; }

        [PersistenceProperty("ESPFURO")]
        public int EspFuro { get; set; }

        [PersistenceProperty("CUSTO")]
        public decimal Custo { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRBENEF", DirectionParameter.InputOptional)]
        public string DescrBenef { get; set; }

        [PersistenceProperty("TIPOCALCBENEF", DirectionParameter.InputOptional)]
        public int TipoCalculoBenef { get; set; }

        #endregion
    }
}