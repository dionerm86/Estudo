using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações do relatório de lista de pedidos.
        /// </summary>
        public static class RelatorioListaPedidos
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            public static string ExibirRelatorioListaPedidosPaisagem
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRelatorioListaPedidosPaisagem) ? "Relatorios/rptListaPedidosPaisagem.rdlc" : "Relatorios/rptListaPedidos.rdlc"; }
            }

            /// <summary>
            /// Define que será exibido os totais dos vendedores associados ao cliente
            /// </summary>
            public static bool ExibirTotaisVendedorCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotaisVendedorCliente); }
            }
        }
    }
}
