using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteApuracaoIPIDAO))]
    [PersistenceClass("sped_ajustes_apuracoes_ipi")]
    public class AjusteApuracaoIPI : Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIpi
    {
        #region Propriedades

        [PersistenceProperty("ID", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("INDICADORTIPOAJUSTE")]
        public uint IndicadorTipoAjuste { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("CODAJUSTE")]
        public int CodAjuste { get; set; }

        [PersistenceProperty("INDICADORORIGEM")]
        public uint IndicadorOrigem { get; set; }

        [PersistenceProperty("NUMERODOCUMENTO")]
        public string NumeroDocumento { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public uint TipoImposto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoIndicadorOrigem
        {
            get
            {
                switch (IndicadorOrigem)
                {
                    case (int)ConfigEFD.IndicadorOrigemDocumentoEnum.ProcessoJudicial: return "Processo Judicial";
                    case (int)ConfigEFD.IndicadorOrigemDocumentoEnum.ProcessoAdministrativo: return "Processo Administrativo";
                    case (int)ConfigEFD.IndicadorOrigemDocumentoEnum.PER_DCOMP: return "PER/DCOMP";
                    case (int)ConfigEFD.IndicadorOrigemDocumentoEnum.Outros: return "Outros";
                }

                return "";
            }
        }

        public string DescricaoIndicadorTipoAjuste
        {
            get
            {
                switch (IndicadorTipoAjuste)
                {
                    case (int)ConfigEFD.TipoAjusteEnum.AjusteCredito: return "Ajuste a Crédito";
                    case (int)ConfigEFD.TipoAjusteEnum.AjusteDebito: return "Ajuste a Débito";
                }

                return "";
            }
        }

        public string DescricaoTipoImposto
        {
            get
            {
                switch (TipoImposto)
                {
                    case (int)ConfigEFD.TipoImpostoEnum.ICMS: return "ICMS";
                    case (int)ConfigEFD.TipoImpostoEnum.ICMSST: return "ICMS ST";
                    case (int)ConfigEFD.TipoImpostoEnum.IPI: return "IPI";
                }

                return "";
            }
        }

        public string DescricaoCodAjuste
        {
            get { return DataSourcesEFD.Instance.GetDescrCodAjusteIpi(CodAjuste); }
        }

        #endregion

        #region IAjusteApuracaoIpi Members

        Sync.Fiscal.Enumeracao.AjusteApuracaoIpi.IndicadorTipoAjuste Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIpi.IndicadorTipoAjuste
        {
            get { return (Sync.Fiscal.Enumeracao.AjusteApuracaoIpi.IndicadorTipoAjuste)IndicadorTipoAjuste; }
        }

        Sync.Fiscal.Enumeracao.AjusteApuracaoIpi.IndicadorOrigemDocumento Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIpi.IndicadorOrigemDocumento
        {
            get { return (Sync.Fiscal.Enumeracao.AjusteApuracaoIpi.IndicadorOrigemDocumento)IndicadorOrigem; }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteApuracaoIpi.CodigoAjuste
        {
            get { return CodAjuste; }
        }

        #endregion
    }
}