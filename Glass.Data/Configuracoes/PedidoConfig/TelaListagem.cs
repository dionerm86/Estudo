using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de listagem de pedidos.
        /// </summary>
        public static class TelaListagem
        {
            /// <summary>
            /// A empresa exibe as impressões dos projetos do PCP se o
            /// pedido tiver conferência gerada?
            /// </summary>
            public static bool UsarImpressaoProjetoPcp
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarImpressaoProjetoPcp); }
            }

            /// <summary>
            /// Define que apenas pedidos nas situações: Ativo, Em Conferência, Ativo/Conferência e Conferido sejam exibidos para os vendedores
            /// </summary>
            public static bool ExibirApenasPedidosComercialVendedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirApenasPedidosComercialVendedor); }
            }

            /// <summary>
            /// Define que apenas o administrador tenha permissão de visualizar o link de totais e o gráfico diário
            /// </summary>
            public static bool ApenasAdminVisualizaTotais
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminVisualizaTotais); }
            }

            /// <summary>
            /// Define que a linha do pedido ficará azul caso o mesmo esteja pronto
            /// </summary>
            public static bool ExibirLinhaAzulSePedidoPronto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLinhaAzulSePedidoPronto); }
            }

            /// <summary>
            /// Define que a linha do pedido ficará preta caso o mesmo seja de revenda
            /// </summary>
            public static bool ExibirLinhaPretaSeRevenda
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLinhaPretaSeRevenda); }
            }

            /// <summary>
            /// Define que a linha do pedido ficará vermelha caso o mesmo esteja pendente ou tenha alteração no PCP
            /// </summary>
            public static bool ExibirLinhaVermelhaSePendenteOuTemAlteracaoPCP
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLinhaVermelhaSePendenteOuTemAlteracaoPCP); }
            }

            /// <summary>
            /// Define a cod da linha do pedido caso o mesmo seja de importação ou gerado pelo e-commerce
            /// </summary>
            public static string CorLinhaSeImportadoOuGeradoParceiro
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CorLinhaSeImportadoOuGeradoParceiro); }
            }

            /// <summary>
            /// Define se no e-commerce irá aparecer a situação de produção "Pendente" se a situação for "Etiqueta não impresa"
            /// </summary>
            public static bool ExibirSituacaoPendenteECommerce
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirSituacaoPendenteECommerce); }
            }

            /// <summary>
            /// Define que auxiliar administrativo altera pedido somente da loja dele
            /// </summary>
            public static bool AuxAdministrativoAlteraPedidoLojaDele
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AuxAdministrativoAlteraPedidoLojaDele); }
            }
        }
    }
}
