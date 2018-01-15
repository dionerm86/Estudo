using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class Beneficiamentos
    {
        public static class ControleBeneficiamento
        {
            /// <summary>
            /// Nome da função de JavaScript que define o cálculo do valor adicional.
            /// </summary>
            public static string NomeFuncaoJavascriptCalculoValorAdicional
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.NomeFuncaoJavascriptCalculoValorAdicional); }
            }

            /// <summary>
            /// Define se os controles de Lista de seleção por quantidade e quantidade podem ter como cálculo somente o tipo "Quantidade".
            /// </summary>
            public static bool BloquearControleQuantidadeCalculoQuantidade
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearControleQuantidadeCalculoQuantidade); }
            }
        }
    }
}
