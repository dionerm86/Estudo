using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class PCPConfig
    {
        public class TelaPedidoPcp
        {
            /// <summary>
            /// Define que será permitido finalizar o pcp mesmo que tenha ficado com um valor maior que o pedido original tendo pagto antecipado.
            /// </summary>
            public static bool PermitirFinalizarComDiferencaEPagtoAntecip
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirFinalizarComDiferencaEPagtoAntecip); }
            }
        }
    }
}
