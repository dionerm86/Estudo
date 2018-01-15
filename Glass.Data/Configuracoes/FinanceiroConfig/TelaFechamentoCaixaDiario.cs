using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class TelaFechamentoCaixaDiario
        {
            /// <summary>
            /// O caixa diário (ou financeiro) pode alterar a data de consulta da tela?
            /// </summary>
            public static bool PermitirCaixaAlterarDataConsulta
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirCaixaAlterarDataConsulta); }
            }

            /// <summary>
            /// Exibir o filtro por funcionário na tela de fechamento?
            /// </summary>
            public static bool FiltroFuncionarioCaixaDiario
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltroFuncionarioCaixaDiario); }
            }
        }
    }
}