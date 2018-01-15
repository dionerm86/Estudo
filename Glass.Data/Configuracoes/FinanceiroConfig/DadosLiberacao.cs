using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class DadosLiberacao
        {
            /// <summary>
            /// Retorna o número de vias de impressão de nota promissória.
            /// </summary>
            public static int NumeroViasNotaPromissoria
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroViasNotaPromissoria); }
            }

            /// <summary>
            /// Define se na impressão da liberação será exibido a descrição da parcela utilizada na liberação ao invés das datas.
            /// </summary>
            public static bool ExibirDescricaoParcelaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDescricaoParcelaLiberacao); }
            }

            /// <summary>
            /// Define se deve exibir o número da etiqueta na impressão da liberação
            /// </summary>
            public static bool ExibirNumeroEtiquetaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirNumeroEtiquetaLiberacao); }
            }

            /// <summary>
            /// Define que apenas as 4 primeiras etiquetas da peça serão exibidas na liberação
            /// </summary>
            public static bool ExibirAsQuatroPrimeirasEtiquetasNaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirAsQuatroPrimeirasEtiquetasNaLiberacao); }
            }

            /// <summary>
            /// Define que liberações à prazo sempre serão somadas na coluna À Prazo, 
            /// independente da forma de pagto escolhida ao criar a liberação, no relatório de movimentação de liberações
            /// </summary>
            public static bool LiberacoesAPrazoFormaPagtoAPrazoMovLib
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.LiberacoesAPrazoFormaPagtoAPrazoMovLib); }
            }

            /// <summary>
            /// Define se o campo acréscimo será exibido ao liberar pedidos
            /// </summary>
            public static bool ExibirAcrescimoNaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirAcrescimoNaLiberacao); }
            }

            /// <summary>
            /// Define se pedidos de revenda serão marcados como entregue ao liberá-lo, 
            /// desde que esteja marcado para dar saída de box ao liberar e separa pedido venda de revenda
            /// </summary>
            public static bool MarcaPedidoRevendaEntregueAoLiberar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MarcaPedidoRevendaEntregueAoLiberar); }
            }

            /// <summary>
            /// Define se será bloqueada liberação de pedidos com pedidos de lojas diferentes.
            /// </summary>
            public static bool PermitirLiberacaoPedidosLojasDiferentes
            {
                get
                {
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirLiberacaoPedidosLojasDiferentes);
                }
            }
        }
    }
}
