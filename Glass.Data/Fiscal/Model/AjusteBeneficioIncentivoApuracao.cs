using System;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteBeneficioIncentivoApuracaoDAO))]
    [PersistenceClass("sped_ajuste_beneficio_incentivo_apuracao")]
    public class AjusteBeneficioIncentivoApuracao : Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivoApuracao
    {
        #region Propriedades

        [PersistenceProperty("ID", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("IDAJBENINC")]
        public uint IdAjBenInc { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("OBS")]
        public string Observacao { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        [PersistenceProperty("UF")]
        public string Uf { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("CODIGO", DirectionParameter.InputOptional)]
        public string Codigo { get; set; }

        [PersistenceProperty("CODIGODESCRICAO", DirectionParameter.InputOptional)]
        public string CodigoDescricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoAjuste
        {
            get
            {
                return Codigo + " - " + Data.ToString("dd/MM/yyyy") + " - " + Observacao;
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
        #endregion

        #region IAjusteBeneficioIncentivoApuracao Members

        int Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivoApuracao.Codigo
        {
            get { return (int)Id; }
        }

        int Sync.Fiscal.EFD.Entidade.IAjusteBeneficioIncentivoApuracao.CodigoAjusteBeneficioApuracao
        {
            get { return (int)IdAjBenInc; }
        }

        #endregion
    }
}