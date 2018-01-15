using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class ContasPagarReceber
        {
            public static string DescricaoContaContabil
            {
                get
                {
                    return "DP";
                }
            }

            public static string DescricaoContaNaoContabil
            {
                get
                {
                    return "CP";
                }
            }

            public static string DescricaoContaCupomFiscal
            {
                get
                {
                    return "CF";
                }
            }
        }
    }
}
