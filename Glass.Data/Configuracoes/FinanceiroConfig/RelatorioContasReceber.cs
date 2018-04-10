using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioContasReceber
        {
            /// <summary>
            /// Verifica se deve ser exibido o campo PedCli no relatório
            /// </summary>
            public static bool ExibirPedCli
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedCli); }
            }
        }
    }
}
