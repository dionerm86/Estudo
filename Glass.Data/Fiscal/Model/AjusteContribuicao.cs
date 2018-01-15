using System;
using Glass.Data.EFD;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteContribuicaoDAO))]
    [PersistenceClass("ajuste_contribuicao")]
    public class AjusteContribuicao : Sync.Fiscal.EFD.Entidade.IAjusteContribuicao
    {
        #region Propriedades

        [PersistenceProperty("IDAJUSTECONT", PersistenceParameterType.IdentityKey)]
        public uint IdAjusteCont { get; set; }

        [PersistenceProperty("CODCREDCONT")]
        public int CodCredCont { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("TIPOAJUSTE")]
        public int TipoAjuste { get; set; }

        [PersistenceProperty("VALORAJUSTE")]
        public decimal ValorAjuste { get; set; }

        [PersistenceProperty("CODIGOAJUSTE")]
        public int CodigoAjuste { get; set; }

        [PersistenceProperty("NUMERODOCUMENTO")]
        public string NumeroDocumento { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATAAJUSTE")]
        public DateTime DataAjuste { get; set; }

        [PersistenceProperty("FONTEAJUSTE")]
        public int FonteAjuste { get; set; }

        [PersistenceProperty("ALIQIMPOSTO")]
        public float AliqImposto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }

        public string DescrTipoAjuste
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoAjuste(TipoAjuste); }
        }

        public string DescrCodigoAjuste
        {
            get { return DataSourcesEFD.Instance.GetDescrCodAjusteContCred(CodigoAjuste); }
        }

        public string DescrCodCredCont
        {
            get
            {
                return FonteAjuste == (int)Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste.Credito ? 
                    DataSourcesEFD.Instance.GetDescrCodCred(CodCredCont) :
                    DataSourcesEFD.Instance.GetDescrCodCont(CodCredCont);
            }
        }

        public string DescrFonteAjuste
        {
            get { return DataSourcesEFD.Instance.GetDescrFonteAjuste(FonteAjuste); }
        }

        #endregion

        #region IAjusteContribuicao Members

        Sync.Fiscal.Enumeracao.AjusteContribuicao.TipoAjuste Sync.Fiscal.EFD.Entidade.IAjusteContribuicao.TipoAjuste
        {
            get { return (Sync.Fiscal.Enumeracao.AjusteContribuicao.TipoAjuste)TipoAjuste; }
        }

        Sync.Fiscal.EFD.DataSources.CodAjusteContCred Sync.Fiscal.EFD.Entidade.IAjusteContribuicao.CodigoAjuste
        {
            get { return (Sync.Fiscal.EFD.DataSources.CodAjusteContCred)CodigoAjuste; }
        }

        #endregion
    }
}