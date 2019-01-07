using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class EstoqueConfig
    {
        /// <summary>
        /// Define se a empresa faz a entrada de estoque manualmente.
        /// </summary>
        public static bool EntradaEstoqueManual
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EntradaEstoqueManual); }
        }

        /// <summary>
        /// Define se será subtraído do valor da movimentação no extrato de estoque fiscal o valor do ICMS de operação própria.
        /// </summary>
        public static bool AbaterICMSDoTotalProdNfMovEstoqueFiscal
        {
        get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AbaterICMSDoTotalProdNfMovEstoqueFiscal); }
        }
        

        /// <summary>
        /// A empresa controla estoque de vidros de clientes?
        /// </summary>
        public static bool ControlarEstoqueVidrosClientes
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControlarEstoqueVidrosClientes); }
        }

        /// <summary>
        /// Verifica se a empresa tem acesso à saída de estoque por volume
        /// </summary>
        public static bool SaidaEstoqueVolume
        {
            get
            {
                if (!OrdemCargaConfig.UsarControleOrdemCarga)
                    return false;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.SaidaEstoqueVolume);
            }
        }

        /// <summary>
        /// Define se a empresa considera o valor total do produto da nota fiscal ao creditar o estoque fiscal do produto base.
        /// </summary>
        public static bool ConsiderarTotalProdNfMovEstoqueFiscal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarTotalProdNfMovEstoqueFiscal); }
        }

        public static bool ExibirQuantidadeProdutosAbaixoOuNoEstoqueMinimoTopoTela
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirQuantidadeProdutosAbaixoOuNoEstoqueMinimoTopoTela); }
        }

        /// <summary>
        /// Define que será permitido selecionar o funcionário que irá usar o pedido interno
        /// </summary>
        public static bool PermitirAlterarFuncionarioPedidoInterno
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirAlterarFuncionarioPedidoInterno); }
        }

        /// <summary>
        /// Define que serão exibidos os pedidos da movimentação de estoque fiscal relacionados à nota
        /// </summary>
        public static bool ExibirPedidosEstoqueFiscal
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosEstoqueFiscal); }
        }

        /// <summary>
        /// Define que será permitido trocar o funcionário ao cadastrar troca/devolução, mesmo se não for administrador
        /// </summary>
        public static bool PermitirAlterarFuncionarioTrocaDevolucao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirAlterarFuncionarioTrocaDevolucao); }
        }

        /// <summary>
        /// Define se os valores serão exibidos com um sinal negativo nos produtos trocados
        /// </summary>
        public static bool ExibirValoresNegativosProdutoTrocado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValoresNegativosProdutoTrocado); }
        }

        /// <summary>
        /// Define se será permitido alterar o crédito gerado numa troca/devolução desde que o valor excedente seja 0 (zero)
        /// </summary>
        public static bool PermitirAlterarCreditoGerado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirAlterarCreditoGerado); }
        }
    }
}