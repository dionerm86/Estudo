using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class MensagemConfig
    {
        /// <summary>
        /// Permitir excluir mensagens?
        /// </summary>
        public static bool ImpedirEnvioDeMensagemAoDescontarParcela
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirEnvioDeMensagemAoDescontarParcela); }
        }
    }
}
