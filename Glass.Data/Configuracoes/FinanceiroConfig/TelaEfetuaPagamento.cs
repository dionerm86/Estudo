using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class TelaEfetuaPagamento
        {
            /// <summary>
            /// Exibir relatório do pagamento ao finalizar?
            /// </summary>
            public static bool ExibirRelatorioAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRelatorioAoFinalizar); }
            }
        }
    }
}
