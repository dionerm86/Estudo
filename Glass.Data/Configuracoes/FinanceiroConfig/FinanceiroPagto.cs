using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class FinanceiroPagto
        {
            /// <summary>
            /// Permite apenas que compras sejam quitadas a loja do funcionário logado
            /// </summary>
            public static bool ImpedirPagamentoPorLoja
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirPagamentoPorLoja); }
            }

            /// <summary>
            /// Define que o usuário só visualizará as compras da loja dele
            /// </summary>
            public static bool SepararListagemCompras
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SepararListagemCompras); }
            }

            /// <summary>
            /// Indica a loja padrão para a compra.
            /// </summary>
            public static uint? CompraLojaPadrao
            {
                get
                {
                    var config = Config.GetConfigItem<uint>(Config.ConfigEnum.CompraLojaPadrao);

                    return config == 0 ? null : (uint?)config;
                }
            }

            public static bool ExibirRazaoSocialContasPagarPagas
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRazaoSocialContasPagarPagas); }
            }

            /// <summary>
            /// Define que a observação da compra será exibida junto com a descrição da conta a pagar/paga
            /// </summary>
            public static bool ExibirObsCompraContasPagar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsCompraContasPagar); }
            }

            /// <summary>
            /// Define que a observação do imposto/serviço será exibido na conta a pagar/paga
            /// </summary>
            public static bool ExibirObsImpostoServico
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsImpostoServico); }
            }

            /// <summary>
            /// Define que o icms será subtraído do total do pedido para cálculo da comissão
            /// </summary>
            public static bool SubtrairICMSCalculoComissao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SubtrairICMSCalculoComissao); }
            }
        }
    }
}
