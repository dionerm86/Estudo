using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model.EFD
{
    [PersistenceBaseDAO(typeof(InfoAdicCreditoDAO))]
    [PersistenceClass("info_adic_creditos_efd")]
    public class InfoAdicCredito : Sync.Fiscal.EFD.Entidade.IInfoAdicionalCredito
    {
        #region Propriedades

        [PersistenceProperty("IDINFOADICCRED", PersistenceParameterType.IdentityKey)]
        public uint IdInfoAdicCredito { get; set; }

        [PersistenceProperty("CODCRED")]
        public int CodCred { get; set; }

        [PersistenceProperty("PERIODO")]
        public string Periodo { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("VALORCREDPERRESANT")]
        public decimal ValorCredPerResAnt { get; set; }

        [PersistenceProperty("VALORCREDDECCOMPANT")]
        public decimal ValorCredDeclCompAnt { get; set; }

        [PersistenceProperty("VALORCREDDESCANT")]
        public decimal ValorCredDescAnt { get; set; }

        [PersistenceProperty("VALORCREDPERRES")]
        public decimal ValorCredPerRes { get; set; }

        [PersistenceProperty("VALORCREDDECCOMP")]
        public decimal ValorCredDeclComp { get; set; }

        [PersistenceProperty("VALORCREDTRANS")]
        public decimal ValorCredTransf { get; set; }

        [PersistenceProperty("VALORCREDOUT")]
        public decimal ValorCredOutro { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrCodCred
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCred(CodCred); }
        }

        public string DescrTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }

        #endregion
    }
}