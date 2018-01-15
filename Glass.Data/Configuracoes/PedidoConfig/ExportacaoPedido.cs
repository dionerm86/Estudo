using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class ExportacaoPedido
        {
            /// <summary>
            /// A URL do sistema WebGlass pode ser alterada no fornecedor?
            /// </summary>
            public static bool AlterarUrlWebGlassFornec
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarUrlWebGlassFornec); }
            }

            /// <summary>
            /// Define que será mantido o tipo de entrega do pedido
            /// </summary>
            public static bool ManterTipoEntregaPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ManterTipoEntregaPedido); }
            }

            /// <summary>
            /// Define que pedidos liberados/confirmados não aparecerão para serem exportados.
            /// </summary>
            public static bool EsconderPedidosLiberados
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderPedidosLiberados); }
            }

            /// <summary>
            /// Define que todos produtos não importados serão buscados para exportação, caso contrário, busca apenas produtos que ainda não foram exportados do grupo vidro e que não seja produto de estoque
            /// </summary>
            public static bool BuscarTodosProdutosNaoExportados
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarTodosProdutosNaoExportados); }
            }

            /// <summary>
            /// Define o intervalo de dias, à partir da data atual, do período do pedido filtrado por padrão na tela de Exportar Pedido.
            /// </summary>
            public static int IntervaloDiasExportarPedido
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.IntervaloDiasExportarPedido); }
            }
        }
    }
}
