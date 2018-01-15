using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(InfoDiferimentoDAO))]
    [PersistenceClass("info_diferimento")]
    public class InfoDiferimento : Sync.Fiscal.EFD.Entidade.IInfoDiferimento
    {
        #region Propriedades

        [PersistenceProperty("IDINFODIFERIMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdInfoDiferimento { get; set; }

        [PersistenceProperty("CNPJ")]
        public string Cnpj { get; set; }

        [PersistenceProperty("VALORRECEBIDO")]
        public decimal ValorRecebido { get; set; }

        [PersistenceProperty("VALORNAORECEBIDO")]
        public decimal ValorNaoRecebido { get; set; }

        [PersistenceProperty("VALORCONT")]
        public decimal ValorContribuicao { get; set; }

        [PersistenceProperty("VALORCREDITO")]
        public decimal ValorCredito { get; set; }

        [PersistenceProperty("CODCRED")]
        public int CodCred { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("CODCONT")]
        public int CodCont { get; set; }

        [PersistenceProperty("ALIQIMPOSTO")]
        public float AliqImposto { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrCodCred
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCred(CodCred); }
        }

        public string DescrCodCont
        {
            get { return DataSourcesEFD.Instance.GetDescrCodCont(CodCont); }
        }

        public string DescrTipoImposto
        {
            get { return DataSourcesEFD.Instance.GetDescrTipoImposto(TipoImposto); }
        }

        #endregion

        #region IInfoDiferimento Members

        Sync.Fiscal.Enumeracao.CodigoTipoCredito Sync.Fiscal.EFD.Entidade.IInfoDiferimento.CodigoCredito
        {
            get { return (Sync.Fiscal.Enumeracao.CodigoTipoCredito)CodCred; }
        }

        string Sync.Fiscal.EFD.Entidade.IInfoDiferimento.CNPJ
        {
            get { return Cnpj; }
        }

        #endregion
    }
}