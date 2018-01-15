using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public class CaixaGeral
        {
            /// <summary>
            /// Verifica se o funcionário financeiro pode ver o saldo total do caixa geral
            /// </summary>
            public static bool CxGeralSaldoTotal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CxGeralSaldoTotal); }
            }

            /// <summary>
            /// Mostrar total cumulativo do caixa geral
            /// </summary>
            public static bool CxGeralTotalCumulativo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CxGeralTotalCumulativo); }
            }

            /// <summary>
            /// Define se será exibido débito/crédito no caixa geral
            /// </summary>
            public static bool ExibirDebitoCredito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDebitoCredito); }
            }
        }
    }
}
