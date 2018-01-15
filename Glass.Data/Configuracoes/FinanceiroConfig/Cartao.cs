using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class Cartao
        {
            /// <summary>
            /// Retorna a taxa de juros usada pelo Construcard.
            /// </summary>
            public static float TaxaJurosConstrucard
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.TaxaJurosConstrucard); }
            }

            /// <summary>
            /// Define se a empresa cobra os juros do cartão do cliente.
            /// </summary>
            public static bool CobrarJurosCartaoCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CobrarJurosCartaoCliente); }
            }

            /// <summary>
            /// Verifica se a empresa trabalha com Juros/Parcela de cartão
            /// </summary>
            public static bool PedidoJurosCartao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PedidoJurosCartao); }
            }

            /// <summary>
            /// Verifica se a empresa quita parcelas do cartão de débito manualmente.
            /// </summary>
            public static bool QuitarParcCartaoDebito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.QuitarParcCartaoDebito); }
            }

            /// <summary>
            /// Define se movimentação de cartão de crédito irá gerar movimentação no caixa geral e no diário
            /// </summary>
            public static bool CartaoMovimentaCxGeralDiario
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CartaoMovimentaCxGeralDiario); }
            }

            /// <summary>
            /// Define a quantidade de dias que deverá ser somada à data atual para lançar a movimentação bancária do cartão de débito.
            /// </summary>
            public static int QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito); }
            }

            /// <summary>
            /// Define a quantidade de dias que deverá ser somada à data atual para lançar a movimentação bancária do cartão de crédito.
            /// </summary>
            public static int QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito); }
            }
        }
    }
}
