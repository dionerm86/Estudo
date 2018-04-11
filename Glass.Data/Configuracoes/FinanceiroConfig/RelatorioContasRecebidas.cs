using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class RelatorioContasRecebidas
        {
            public static bool ExibirPedidos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidos); }
            }

            /// <summary>
            /// Define que a comissão será exibida na impressão do relatório de contas recebidas e na tela
            /// </summary>
            public static bool ExibirComissao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirComissao); }
            }
        }
    }
}