using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static class EmailConfig
    {
        /// <summary>
        /// Define se o pedido será enviado como anexo nos e-mails confirmado/pronto.
        /// </summary>
        public static DataSources.TipoEnvioAnexoPedidoEmail EnviarPedidoAnexoEmail
        {
            get { return Config.GetConfigItem<DataSources.TipoEnvioAnexoPedidoEmail>(Config.ConfigEnum.EnviarPedidoAnexoEmail); }
        }

        /// <summary>
        /// Deverá ser enviado e-mail para um administrador se houver um desconto
        /// maior que o configurado em um orçamento/pedido?
        /// </summary>
        public static bool EnviarEmailAdministradorDescontoMaior
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailAdministradorDescontoMaior); }
        }

        /// <summary>
        /// O código do administrador que receberá o e-mail se houver um desconto
        /// maior que o configurado em um orçamento/pedido.
        /// </summary>
        public static uint? AdministradorEnviarEmailDescontoMaior
        {
            get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.AdministradorEnviarEmailDescontoMaior); }
        }

        /// <summary>
        /// Define se serão considerados pedidos de Reposição e Garantia no total dos pedidos emitidos no mês.
        /// </summary>
        public static bool ConsiderarReposicaoGarantiaTotalPedidosEmitidos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarReposicaoGarantiaTotalPedidosEmitidos); }
        }

        /// <summary>
        /// Define quais usuários receberão o e-mail informando que o desconto do pedido é maior que o permitido.
        /// </summary>
        public static string UsuariosQueDevemReceberEmailDescontoMaior
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.UsuariosQueDevemReceberEmailDescontoMaior); }
        }

        /// <summary>
        /// Define o texto que será enviado por email ao finalizar o PCP
        /// </summary>
        public static string TextoEmailPedidoFinalizadoPcp
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoEmailPedidoFinalizadoPcp); }
        }

        /// <summary>
        /// Define o texto que será enviado por email quando o pedido de balcão ficar pronto
        /// </summary>
        public static string TextoEmailPedidoProntoBalcao
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoEmailPedidoProntoBalcao); }
        }

        /// <summary>
        /// Define o texto que será enviado por email quando o pedido de entrega ficar pronto
        /// </summary>
        public static string TextoEmailPedidoProntoEntrega
        {
            get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoEmailPedidoProntoEntrega); }
        }

        /// <summary>
        /// O código do administrador que receberá o e-mail se houver alteração no preço de venda do pedido.
        /// </summary>
        public static uint? AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado
        {
            get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado); }
        }
    }
}
