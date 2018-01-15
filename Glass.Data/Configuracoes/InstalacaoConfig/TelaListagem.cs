using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class InstalacaoConfig
    {
        public class TelaListagem
        {
            /// <summary>
            /// O filtro por loja deve vir selecionada a loja do funcionário
            /// como padrão ao abrir a tela?
            /// </summary>
            public static bool FiltrarPorLojaComoPadrao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltrarPorLojaComoPadrao); }
            }

            /// <summary>
            /// Define se será possível alterar a situação das instalações manualmente, desde que a mesma esteja em uma destas situações: Aberta, Agendar, Colagem, Depto Técnico
            /// </summary>
            public static bool PermitirAlterarSituacaoManualmente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirAlterarSituacaoManualmente); }
            }
        }
    }
}
