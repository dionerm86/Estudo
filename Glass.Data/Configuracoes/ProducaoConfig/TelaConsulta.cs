using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class ProducaoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de consulta de produção.
        /// </summary>
        public static class TelaConsulta
        {
            /// <summary>
            /// Os links de impressão devem ser escondidos para vendedores?
            /// </summary>
            public static bool EsconderLinksImpressaoParaVendedores
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderLinksImpressaoParaVendedores); }
            }

            /// <summary>
            /// A lista de peças na produção deve ser escondida para vendedores?
            /// </summary>
            public static bool EsconderPecasParaVendedores
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderPecasParaVendedores); }
            }

            /// <summary>
            /// O número da etiqueta deve aparecer no início da tabela?
            /// </summary>
            public static bool ExibirNumeroEtiquetaNoInicioDaTabela
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirNumeroEtiquetaNoInicioDaTabela); }
            }

            /// <summary>
            /// Define se a tela de consulta produção virá vazia caso nenhum filtro tenha sido usado
            /// </summary>
            public static bool TelaVaziaPorPadrao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.TelaVaziaPorPadrao); }
            }

            /// <summary>
            /// Define que será buscado o nome fantasia do cliente ao invés da razão social na consulta produção
            /// </summary>
            public static bool BuscarNomeFantasiaConsultaProducao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarNomeFantasiaConsultaProducao); }
            }

            /// <summary>
            /// Define que a tela de consulta será filtrada pelo numSeq do setor
            /// </summary>
            public static bool OrdenarPeloNumSeqSetor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenarPeloNumSeqSetor); }
            }

            /// <summary>
            /// Define que na tela de consulta ao filtrar pelo PedCli irá filtrar também pelo ambiente do produto
            /// </summary>
            public static bool FiltrarAmbienteAoFiltrarPedCli
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltrarAmbienteAoFiltrarPedCli); }
            }

            /// <summary>
            /// Define se a opção de imprimir o pedido aparecerá para quem não é administrador na tela de consulta produção
            /// </summary>
            public static bool ExibirImpressaoPedidoTelaConsulta
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirImpressaoPedidoTelaConsulta); }
            }
        }
    }
}
