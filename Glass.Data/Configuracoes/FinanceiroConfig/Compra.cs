using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class Compra
        {
            /// <summary>
            /// Número de parcelas configuráveis para a Compra.
            /// </summary>
            public static int NumeroParcelasCompra
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroParcelasCompra); }
            }

            /// <summary>
            /// Define se a empresa utiliza o controle de geração de nota fiscal para produtos contábeis da compra.
            /// </summary>
            public static bool UsarControleCompraContabilNF
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleCompraContabilNF); }
            }

            /// <summary>
            /// Verifica se é para calcular o m² nos produtos da compra
            /// </summary>
            /// <returns></returns>
            public static bool CompraCalcMult5
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CompraCalcMult5); }
            }

            /// <summary>
            /// O relatório de compra sai sem valor se o fornecedor for a própria empresa?
            /// </summary>
            public static bool CompraSemValores
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CompraSemValores); }
            }

            /// <summary>
            /// Define se deve ser usado o controle de finalizacao de compra
            /// </summary>
            public static bool UsarControleFinalizacaoCompra
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleFinalizacaoCompra); }
            }
        }
    }
}
