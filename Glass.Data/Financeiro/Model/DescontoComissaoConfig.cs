using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DescontoComissaoConfigDAO))]
	[PersistenceClass("desconto_comissao_config")]
	public class DescontoComissaoConfig
    {
        #region Propriedades

        [PersistenceProperty("IDDESCONTOCOMISSAOCONFIG", PersistenceParameterType.IdentityKey)]
        public uint IdDescontoComissaoConfig { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [PersistenceProperty("FAIXAUM")]
        public decimal FaixaUm { get; set; }

        [PersistenceProperty("FAIXADOIS")]
        public decimal FaixaDois { get; set; }

        [PersistenceProperty("FAIXATRES")]
        public decimal FaixaTres { get; set; }

        [PersistenceProperty("FAIXAQUATRO")]
        public decimal FaixaQuatro { get; set; }

        [PersistenceProperty("FAIXACINCO")]
        public decimal FaixaCinco { get; set; }

        [PersistenceProperty("PERCFAIXAUM")]
        public float PercFaixaUm { get; set; }

        [PersistenceProperty("PERCFAIXADOIS")]
        public float PercFaixaDois { get; set; }

        [PersistenceProperty("PERCFAIXATRES")]
        public float PercFaixaTres { get; set; }

        [PersistenceProperty("PERCFAIXAQUATRO")]
        public float PercFaixaQuatro { get; set; }

        [PersistenceProperty("PERCFAIXACINCO")]
        public float PercFaixaCinco { get; set; }

        #endregion
    }
}