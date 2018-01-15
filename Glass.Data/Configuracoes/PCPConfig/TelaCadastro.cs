using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PCPConfig
    {
        public class TelaCadastro
        {
            /// <summary>
            /// A lista de pedidos espelho deve ser aberta ao finalizar o pedido?
            /// </summary>
            public static bool RedirecionarListaPedidosAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RedirecionarListaPedidosAoFinalizar); }
            }

            /// <summary>
            /// Define se será gerado crédito do valor excedente no PCP
            /// </summary>
            public static bool GerarCreditoValorExcedente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarCreditoValorExcedente); }
            }

            /// <summary>
            /// Define se será perguntado ao usuário se deseja gerar crédito excedente ao finalizar o PCP
            /// </summary>
            public static bool PerguntarGerarCreditoAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PerguntarGerarCreditoAoFinalizar); }
            }
        }
    }
}
