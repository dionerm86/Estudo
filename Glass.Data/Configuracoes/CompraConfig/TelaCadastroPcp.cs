using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class CompraConfig
    {
        public class TelaCadastroPcp
        {
            /// <summary>
            /// Ao cadastrar a compra pergunta se o usuário quer continuar na tela ou redirecionar para a listagem?
            /// </summary>
            public static bool PerguntarContinuarTelaAoCadastrar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PerguntarContinuarTelaAoCadastrar); }
            }

            /// <summary>
            /// Usar o custo do beneficiamento no lugar do valor?
            /// </summary>
            public static bool UsarCustoBeneficiamento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarCustoBeneficiamento); }
            }
        }
    }
}
