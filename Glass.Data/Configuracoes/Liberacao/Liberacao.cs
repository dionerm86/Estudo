using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class Liberacao
    {
        /// <summary>
        /// Verifica se quando um pedido for de mão de obra deverá emitir a via do almoxarifado igual a via do cliente, para que os ambientes que na verdade são peças de vidro apareça na grid.
        /// </summary>
        public static bool UsarViaAlmoxarifadoIgualClienteSeMaoDeObra
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarViaAlmoxarifadoIgualClienteSeMaoDeObra); }
        }

        /// <summary>
        /// Define que apenas administradores podem cancelar liberações
        /// </summary>
        public static bool ApenasAdminCancelaLiberacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminCancelaLiberacao); }
        }

        /// <summary>
        /// Verifica se deve ser mostrado a lista de produtos na via da empresa
        /// no relatorio de liberação
        /// </summary>
        public static bool ExibirProdutosViaEmpresa
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirProdutosViaEmpresa); }
        }

        /// <summary>
        /// Verifica se não deve mostrar o valor da obra na liberação
        /// </summary>
        public static bool NaoMostrarValorObraLiberacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoMostrarValorObraLiberacao); }
        }

        /// <summary>
        /// Enviar e-mail ao liberar pedido?
        /// </summary>
        public static bool EnviarEmailAoLiberarPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarEmailAoLiberarPedido); }
        }

        /// <summary>
        /// Verifica se deve ser bloqueada a liberação de pedidos caso os mesmos possuam parcelas diferentes.
        /// </summary>
        public static bool BloquearLiberacaoParcelasDiferentes
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearLiberacaoParcelasDiferentes); }
        }
    }
}
