using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ControleCreditoEfdDAO))]
    [PersistenceClass("controle_credito_efd")]
    public class ControleCreditoEfd : Sync.Fiscal.EFD.Entidade.IControleCreditoEFD
    {
        #region Propriedades

        [PersistenceProperty("IDCREDITO", PersistenceParameterType.IdentityKey)]
        public uint IdCredito { get; set; }

        [Log("Período")]
        [PersistenceProperty("PERIODOGERACAO")]
        public string PeriodoGeracao { get; set; }

        [Log("Valor Gerado")]
        [PersistenceProperty("VALORGERADO")]
        public decimal ValorGerado { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("CODCRED")]
        public int? CodCred { get; set; }

        [PersistenceProperty("IdLoja")]
        public uint IdLoja { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("VALORUSADOCREDITO", DirectionParameter.InputOptional)]
        public decimal ValorUsadoCredito { get; set; }

        [PersistenceProperty("DescrLoja", DirectionParameter.InputOptional)]
        public string DescrLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        public decimal ValorRestanteCredito
        {
            get { return ValorGerado - ValorUsadoCredito; }
        }

        [Log("Tipo de Imposto")]
        public string DescrTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }

        [Log("Código de Crédito")]
        public string DescrCodCred
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCred(CodCred); }
        }

        #endregion

        #region IControleCreditoEFD Members

        Sync.Fiscal.Enumeracao.TipoImposto Sync.Fiscal.EFD.Entidade.IControleCreditoEFD.TipoImposto
        {
            get { return (Sync.Fiscal.Enumeracao.TipoImposto)TipoImposto; }
        }

        Sync.Fiscal.Enumeracao.CodigoTipoCredito? Sync.Fiscal.EFD.Entidade.IControleCreditoEFD.CodCred
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoTipoCredito?)CodCred; }
        }

        #endregion
    }
}