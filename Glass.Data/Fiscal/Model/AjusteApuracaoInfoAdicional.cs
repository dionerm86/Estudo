using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AjusteApuracaoInfoAdicionalDAO))]
    [PersistenceClass("sped_ajuste_apuracao_info_ad")]
    public class AjusteApuracaoInfoAdicional : Sync.Fiscal.EFD.Entidade.IAjusteApuracaoInfoAdicional
    {
        #region Enumeradores

        public enum IndProcEnum
        {
            Sefaz = 0,
            JusticaFederal,
            JusticaEstadual,
            Outros = 9
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("Id", PersistenceParameterType.IdentityKey)]
        public uint Id { get; set; }

        [PersistenceProperty("IdABIA")]
        public uint IdABIA { get; set; }

        [PersistenceProperty("NumDa")]
        public string NumDa { get; set; }

        [PersistenceProperty("NumProc")]
        public string NumProc { get; set; }

        [PersistenceProperty("IndProc")]
        public int IndProc { get; set; }

        [PersistenceProperty("Proc")]
        public string Proc { get; set; }

        [PersistenceProperty("TxtCompl")]
        public string TxtCompl { get; set; }

        [PersistenceProperty("TIPOIMPOSTO")]
        public int TipoImposto { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescricaoIndProc
        {
            get
            {
                switch (IndProc)
                {
                    case (int)IndProcEnum.JusticaEstadual: return "Justiça Estadual";
                    case (int)IndProcEnum.JusticaFederal: return "Justiça Federal";
                    case (int)IndProcEnum.Outros: return "Outros";
                    case (int)IndProcEnum.Sefaz: return "Sefaz";
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
        
        #endregion
    }
}