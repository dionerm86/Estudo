using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ClienteConfig
    {
        public static class TelaCadastro
        {
            /// <summary>
            /// O campo de funcionário (vendedor) deve ser preenchido ao inserir o cliente?
            /// </summary>
            public static bool ExigirFuncionarioAoInserir
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExigirFuncionarioAoInserir); }
            }

            /// <summary>
            /// Se o CPF for colocado como 999.999.999-99 ou se o CNPJ for 99.999.999/9999-99
            /// o validador deve aceitar como válido?
            /// </summary>
            public static bool PermitirCpfCnpjTudo9AoInserir
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirCpfCnpjTudo9AoInserir); }
            }

            /// <summary>
            /// Os campos de bloqueio de cliente (no caso do pedido estar atrasado, por exemplo)
            /// devem ser exibidos apenas para administradores?
            /// </summary>
            public static bool ExibirIgnorarBloqueioApenasAdministrador
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirIgnorarBloqueioApenasAdministrador); }
            }

            /// <summary>
            /// O campo que controla o percentual do limite comprometido pelos cheques ser de, no máximo, 50%
            /// deve vir marcado ao inserir o cliente?
            /// </summary>
            public static bool MarcarBloquearChequesAoInserir
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MarcarBloquearChequesAoInserir); }
            }

            /// <summary>
            /// O e-mail do cliente é exigido ao inserir ou atualizar?
            /// </summary>
            public static bool ExigirEmailClienteAoInserirOuAtualizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExigirEmailClienteAoInserirOuAtualizar); }
            }

            /// <summary>
            /// Verifica se deve exibir informações financeiras do cadastro do cliente
            /// </summary>
            public static bool ExibirInformacoesFinanceiras
            {
                get
                {
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirInformacoesFinanceiras) || UserInfo.GetUserInfo.TipoUsuario != (int)Utils.TipoFuncionario.Vendedor;
                }
            }
        }
    }
}
