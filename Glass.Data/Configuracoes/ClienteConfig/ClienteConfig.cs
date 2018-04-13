using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class ClienteConfig
    {
        /// <summary>
        /// Ao cadastrar cliente, se o funcionário não for administrador ou financeiro, será cadastrado como inativo
        /// </summary>
        public static bool CadastrarClienteInativo
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CadastrarClienteInativo); }
        }

        /// <summary>
        /// Define que ao entrar na tela de listagem de clientes, serão exibidos por padrão apenas os ativos
        /// </summary>
        public static bool ListarAtivosPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ListarAtivosPadrao); }
        }
    }
}
