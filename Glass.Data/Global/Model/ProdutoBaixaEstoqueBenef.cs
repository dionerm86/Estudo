using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ProdutoBaixaEstoqueBenefDAO))]
    [PersistenceClass("produto_baixa_estoque_benef")]
    public class ProdutoBaixaEstoqueBenef : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPRODBAIXAESTBENEF", PersistenceParameterType.IdentityKey)]
        public int IdProdBaixaEstBenef { get; set; }

        [PersistenceProperty("IdProdBaixaEst")]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int IdProdBaixaEst { get; set; }

        [PersistenceProperty("IDBENEFCONFIG")]
        [PersistenceForeignKey(typeof(BenefConfig), "IdBenefConfig")]
        public int IdBenefConfig { get; set; }

        [PersistenceProperty("QTD")]
        public int Qtd { get; set; }

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

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRBENEF", DirectionParameter.InputOptional)]
        public string DescrBenef { get; set; }

        [PersistenceProperty("TIPOCALCBENEF", DirectionParameter.InputOptional)]
        public int TipoCalculoBenef { get; set; }

        #endregion

    }
}
