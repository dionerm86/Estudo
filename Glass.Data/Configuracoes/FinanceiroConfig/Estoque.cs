using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class Estoque
        {
            /// <summary>
            /// Define se ao confirmar pedido a saída de estoque será feita automaticamente
            /// </summary>
            public static bool SaidaEstoqueAutomaticaAoConfirmar
            {
                get { return !Config.GetConfigItem<bool>(Config.ConfigEnum.SaidaEstoqueManual); }
            }
        }
    }
}
