using Glass.Data.Helper;

namespace Glass.Data.CTeUtils
{
    public static class ConfigCTe
    {
        #region Enumeradores

        public enum TipoAmbienteCte
        {
            Producao = 1,
            Homologacao
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Ambiente que o CT-e será operada
        /// </summary>
        public static TipoAmbienteCte TipoAmbiente
        {
            get
            {
                return Config.GetConfigItem<bool>(Config.ConfigEnum.CTeModoProducao) ?
                    TipoAmbienteCte.Producao : TipoAmbienteCte.Homologacao;
            }
        }

        public static string Modelo
        {
            get { return "57"; }
        }

        #endregion

        #region Versão de Layouts

        public static string VersaoCancelamento
        {
            get { return "2.00"; }
        }

        public static string VersaoConsulta
        {
            get { return "2.00"; }
        }

        public static string VersaoConsCad
        {
            get { return "2.00"; }
        }

        public static string VersaoInutilizacao
        {
            get { return "2.00"; }
        }

        public static string VersaoCte
        {
            get { return "2.00"; }
        }

        public static string VersaoModalRod
        {
            get { return "2.00"; }
        }

        public static string VersaoPedRecibo
        {
            get { return "2.00"; }
        }

        public static string VersaoPedSituacao
        {
            get { return "2.00"; }
        }

        public static string VersaoRetRecepcao
        {
            get { return "2.00"; }
        }

        public static string VersaoStatusServico
        {
            get { return "2.00"; }
        }

        public static string VersaoCabecMsg
        {
            get { return "2.00"; }
        }

        public static string VersaoLoteCte
        {
            get { return "2.00"; }
        }

        #endregion
    }
}
