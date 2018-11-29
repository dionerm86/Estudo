using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    /// <summary>
    /// Representa a configuração da parte de PCP.
    /// </summary>
    public static partial class PCPConfig
    {
        /// <summary>
        /// Obtém o nome do servidro do CalcEngine.
        /// </summary>
        public static string ServidorCalcEngine
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.ServidorCalcEngine); }
        }

        /// <summary>
        /// Obtém a porta de comunicação com o servidor do CalcEngine.
        /// </summary>
        public static int PortaCalcEngine
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.PortaCalcEngine); }
        }

        /// <summary>
        /// Obtém o usuário de acesso ao CalcEngine.
        /// </summary>
        public static string UsuarioCalcEngine
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.UsuarioCalcEngine); }
        }

        /// <summary>
        /// Obtém a senha de acesso ao CalcEngine.
        /// </summary>
        public static string SenhaCalcEngine
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.UsuarioCalcEngine); }
        }
    }
}
