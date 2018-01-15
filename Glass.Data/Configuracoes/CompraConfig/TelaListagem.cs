using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class CompraConfig
    {
        public class TelaListagem
        {
            public static bool ExibirDadosAdicionaisPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDadosAdicionaisPedido); }
            }
        }
    }
}
