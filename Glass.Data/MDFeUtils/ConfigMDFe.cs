using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.MDFeUtils
{
    public  class ConfigMDFe
    {
        #region Enumeradores

        public enum TipoAmbienteMDFe
        {
            Producao = 1,
            Homologacao
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Ambiente que o MDFe será operada
        /// </summary>
        public static TipoAmbienteMDFe TipoAmbiente
        {
            get
            {
                return Config.GetConfigItem<bool>(Config.ConfigEnum.MDFeModoProducao) ?
                    TipoAmbienteMDFe.Producao : TipoAmbienteMDFe.Homologacao;
            }
        }

        public static int Modelo
        {
            get { return 58; }
        }

        #endregion

        #region Versão de Layouts

        public static string VersaoMDFe
        {
            get { return "3.00"; }
        }

        public static string VersaoModalRodoviario
        {
            get { return "3.00"; }
        }

        public static string VersaoEnvioMDFe
        {
            get { return "3.00"; }
        }

        public static string VersaoRecepcao
        {
            get { return "3.00"; }
        }

        public static string VersaoRetornoRecepcao
        {
            get { return "3.00"; }
        }

        public static string VersaoRecepcaoEvento
        {
            get { return "3.00"; }
        }

        public static string VersaoConsulta
        {
            get { return "3.00"; }
        }

        public static string VersaoStatusServico
        {
            get { return "3.00"; }
        }

        public static string VersaoEvento
        {
            get { return "3.00"; }
        }

        public static string VersaoEventoCancelamento
        {
            get { return "3.00"; }
        }

        public static string VersaoEventoEncerramento
        {
            get { return "3.00"; }
        }

        public static string VersaoConsultaNaoEncerrado
        {
            get { return "3.00"; }
        }

        #endregion
    }
}
