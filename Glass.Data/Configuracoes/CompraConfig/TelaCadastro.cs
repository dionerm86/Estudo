using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class CompraConfig
    {
        public class TelaCadastro
        {
            /// <summary>
            /// O relatório da compra deve ser aberto ao finalizar a compra?
            /// </summary>
            public static bool AbrirRelatorioAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AbrirRelatorioAoFinalizar); }
            }
        }
    }
}
