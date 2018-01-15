using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class PedidoConfig
    {
        public static class FormaPagamento
        {
            /// <summary>
            /// Retorna o número de formas de pagamento usadas no sistema.
            /// </summary>
            public static int NumeroFormasPagto
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroFormasPagto); }
            }

            /// <summary>
            /// Retorna a quantidade de parcelas no pedido que a empresa trabalha
            /// </summary>
            /// <returns></returns>
            public static int NumParcelasPedido
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumParcelasPedido); }
            }

            /// <summary>
            /// O pedido exibe as datas das parcelas ao invés do número de parcelas?
            /// </summary>
            public static bool ExibirDatasParcelasPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDatasParcelasPedido); }
            }
        }
    }
}
