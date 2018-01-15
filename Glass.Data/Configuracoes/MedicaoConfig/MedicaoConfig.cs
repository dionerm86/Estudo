using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class MedicaoConfig
    {
        /// <summary>
        /// Define se a medição pode ser cadastrada apenas com clientes cadastrados no sistema
        /// </summary>
        public static bool MedicaoApenasClienteCadastrado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MedicaoApenasClienteCadastrado); }
        }

        /// <summary>
        /// Define se o campo E-mail Cliente deverá ser preenchido ao cadastrar ou atualizar uma medição.
        /// </summary>
        public static bool BloquearCadastroMedicaoSemEmailCliente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearCadastroMedicaoSemEmailCliente); }
        }
    }
}
