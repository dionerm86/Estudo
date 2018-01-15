using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static class MenuConfig
    {
        /// <summary>
        /// Define se a tela de gerar compra de produtos associados à beneficiamentos será mostrada ou não.
        /// </summary>
        public static bool ExibirCompraCaixa
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCompraCaixa); }
        }

        /// <summary>
        /// Define que o consultar produção será exibido no e-Commerce
        /// </summary>
        public static bool ExibirConsultaProducaoECommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirConsultaProducaoECommerce); }
        }

        /// <summary>
        /// Define que o preço de tabela por cliente será exibido no e-Commerce
        /// </summary>
        public static bool ExibiPrecoTabelaECommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibiPrecoTabelaECommerce); }
        }

        /// <summary>
        /// Define se será utilizado o controle de Cartões não identificados
        /// </summary>
        public static bool ExibirCartaoNaoIdentificado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCartaoNaoIdentificado); }
        }
    }
}
