using Glass.Data.Helper;
using System.Drawing;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        /// <summary>
        /// Classe com as configurações da tela de cadastro de pedido.
        /// </summary>
        public static class TelaCadastro
        {
            /// <summary>
            /// Os pedidos que são de clientes de Rota devem ser marcados como entrega?
            /// </summary>
            public static bool MarcarPedidosRotaComoEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MarcarPedidosRotaComoEntrega); }
            }

            /// <summary>
            /// O relatório do pedido deve ser aberto ao finalizar o pedido?
            /// </summary>
            public static bool AbrirImpressaoPedidoAoFinalizar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AbrirImpressaoPedidoAoFinalizar); }
            }

            /// <summary>
            /// O pedido PCP deve ser finalizado ao ser gerado?
            /// </summary>
            public static bool FinalizarConferenciaAoGerarEspelho
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FinalizarConferenciaAoGerarEspelho); }
            }

            /// <summary>
            /// O código interno do produto deve ser mantido no campo ao
            /// inserir um produto?
            /// </summary>
            public static bool ManterCodInternoCampoAoInserirProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ManterCodInternoCampoAoInserirProduto); }
            }

            /// <summary>
            /// O código interno do produto deve ser mantido no campo ao
            /// inserir um produto?
            /// </summary>
            public static Data.Model.Pedido.TipoPedidoEnum TipoPedidoPadrao
            {
                get { return (Data.Model.Pedido.TipoPedidoEnum)System.Enum.Parse(typeof(Data.Model.Pedido.TipoPedidoEnum), Config.GetConfigItem<string>(Config.ConfigEnum.TipoPedidoPadrao)); }
            }

            /// <summary>
            /// Buscar endereço do cliente automaticamente se os campos estiverem vazios?
            /// </summary>
            public static bool BuscarEnderecoClienteSeEstiverVazio
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarEnderecoClienteSeEstiverVazio); }
            }

            /// <summary>
            /// Define que será exibido o valor do m² da chapa ao buscar produto para ser inserido no pedido
            /// </summary>
            public static bool ExibirM2ChapaDeVidro
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirM2ChapaDeVidro); }
            }

            /// <summary>
            /// Define que caso o controle de data de entrega mínima não retorne uma data, busca a data de hoje no campo
            /// </summary>
            public static bool BuscarDataEntregaDeHojeSeDataVazia
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BuscarDataEntregaDeHojeSeDataVazia); }
            }

            /// <summary>
            /// Define se o crédito do cliente será exibido ao buscá-lo durante a inserção do pedido
            /// </summary>
            public static bool ExibirCreditoClienteAoBuscar
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCreditoClienteAoBuscar); }
            }

            /// <summary>
            /// Verifica se pode inserir vidros normais e de composição em um pedido
            /// </summary>
            public static bool PermitirInserirVidroComumComComposicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirInserirVidroComumComComposicao); }
            }
        }
    }
}
