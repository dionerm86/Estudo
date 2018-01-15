using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class MensagemConfig
    {
        public static class TelaMensagens
        {
            /// <summary>
            /// Permitir excluir mensagens?
            /// </summary>
            public static bool ExibirBotaoExcluirMensagem
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirBotaoExcluirMensagem); }
            }

            /// <summary>
            /// Define se será exibido as mensagem na tela principal do Webglass Parceiros
            /// </summary>
            public static bool ExibirMensagemWebGlassParceiros
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirMensagemWebGlassParceiros); }
            }
        }
    }
}
