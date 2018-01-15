using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Instalacao
        {
            /// <summary>
            /// Identifica se a instalação será feita por ambiente
            /// </summary>
            public static bool UsarAmbienteInstalacao
            {
                get { return PedidoConfig.DadosPedido.AmbientePedido && 
                    Config.GetConfigItem<bool>(Config.ConfigEnum.UsarAmbienteInstalacao); }
            }

            /// <summary>
            /// Exibir o valor dos produtos no relatório de instalação?
            /// </summary>
            public static bool ExibirValorProdutosInstalacao
            {
                get { return PedidoConfig.Instalacao.UsarAmbienteInstalacao && 
                    Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValorProdutosInstalacao); }
            }

            /// <summary>
            /// Define se a instalação será gerada automaticamente.
            /// </summary>
            public static bool GerarInstalacaoAutomaticamente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarInstalacaoAutomaticamente); }
            }

            /// <summary>
            /// Define se será permitido gerar instalação manual
            /// </summary>
            public static bool GerarInstalacaoManual
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarInstalacaoManual); }
            }
        }
    }
}
