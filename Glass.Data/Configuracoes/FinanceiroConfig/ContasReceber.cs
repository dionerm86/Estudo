using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class ContasReceber
        {
            /// <summary>
            /// Define se na tela de contas a receber virá filtrado por padrão para incluir contas de arquivo de remessa
            /// </summary>
            public static bool FiltroPadraoIncluirContasArquivoRemessa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltroPadraoIncluirContasArquivoRemessa); }
            }

            /// <summary>
            /// Define se serão exibidos os pedidos da liberação no relatório de débitos do cliente
            /// </summary>
            public static bool ExibirPedidosLiberacaoDebitos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosLiberacaoDebitos); }
            }

            /// <summary>
            /// Chamado 33737
            /// </summary>
            public static bool ForcarInstrucaoPadrao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ForcarInstrucaoPadrao); }
            }

            /// <summary>
            /// Define que os campos para seleção na tela de impressão do boleto ficarão bloqueados caso seja banco do Brasil
            /// </summary>
            public static bool DesabilitarCamposImpessaoBoletoBancoDoBrasil
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DesabilitarCamposImpessaoBoletoBancoDoBrasil); }
            }

            /// <summary>
            /// Define que será exibido o telefone do cliente com o nome nos relatórios
            /// </summary>
            public static bool ExibirTelefoneComNomeCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTelefoneComNomeCliente); }
            }

            /// <summary>
            /// Define que será exibido o telefone e endereço do cliente com o nome nos relatórios
            /// </summary>
            public static bool ExibirTelefoneComNomeClienteEEndereco
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTelefoneComNomeClienteEEndereco); }
            }
        }
    }
}
