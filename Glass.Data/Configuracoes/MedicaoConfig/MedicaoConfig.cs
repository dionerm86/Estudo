using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class MedicaoConfig
    {
        public static bool MedicaoAlterarSituacaoCadastro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MedicaoAlterarSituacaoCadastro); }
        }

        /// <summary>
        /// Define se a medição pode ser cadastrada apenas com clientes cadastrados no sistema
        /// </summary>
        public static bool MedicaoApenasClienteCadastrado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MedicaoApenasClienteCadastrado); }
        }

        /// <summary>
        /// Define se o vendedor que tiver permissão de efetuar medição pode alterar todas as medições
        /// </summary>
        public static bool MedicaoPermissaoAlterarTodos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MedicaoPermissaoAlterarTodos); }
        }
    }
}
