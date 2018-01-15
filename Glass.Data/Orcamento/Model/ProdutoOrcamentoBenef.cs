using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoOrcamentoBenefDAO))]
    [PersistenceClass("produto_orcamento_benef")]
    public class ProdutoOrcamentoBenef
    {
        #region Propriedades

        [PersistenceProperty("IDPRODORCABENEF", PersistenceParameterType.IdentityKey)]
        public uint IdProdOrcaBenef { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        public uint IdBenefConfig { get; set; }

        [PersistenceProperty("IDPROD")]
        public uint IdProd { get; set; }

        [PersistenceProperty("QTD")]
        public int Qtde { get; set; }

        private decimal _valorUnit;

        [PersistenceProperty("VALORUNIT")]
        public decimal ValorUnit
        {
            get { return GenericBenef.GetValorUnit(_valorUnit, Valor, Qtde); }
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

        [PersistenceProperty("PADRAO")]
        public bool Padrao { get; set; }

        [PersistenceProperty("VALORCOMISSAO")]
        public decimal ValorComissao { get; set; }

        [PersistenceProperty("VALORACRESCIMO")]
        public decimal ValorAcrescimo { get; set; }

        [PersistenceProperty("VALORACRESCIMOPROD")]
        public decimal ValorAcrescimoProd { get; set; }

        [PersistenceProperty("VALORDESCONTO")]
        public decimal ValorDesconto { get; set; }

        [PersistenceProperty("VALORDESCONTOPROD")]
        public decimal ValorDescontoProd { get;set; }

        #endregion
    }
}