using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL.EFD;

namespace Glass.Data.Model.EFD
{
    [PersistenceBaseDAO(typeof(InfoAdicNFEExtemporaneaDAO))]
    [PersistenceClass("info_adic_nfe_extemporanea")]
    public class InfoAdicNFEExtemporanea
    {
        #region Propriedades

        [PersistenceProperty("IDINFOADICNFEEXT", PersistenceParameterType.IdentityKey)]
        public uint IdInfoAdicNFEExtemporanea { get; set; }

        [PersistenceProperty("IDNF")]
        public uint IdNFE { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("VALOROUTDEDUCOES")]
        public decimal ValorOutDeducao { get; set; }

        [PersistenceProperty("VALORMULTA")]
        public decimal ValorMulta { get; set; }

        [PersistenceProperty("VALORJUROS")]
        public decimal ValorJuro { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public uint NumeroNFe { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }

        #endregion
    }
}