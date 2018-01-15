using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class CompraConfig
    {
        /// <summary>
        /// Usar tipo de cálculo de NF-e para compra?
        /// </summary>
        public static bool UsarTipoCalculoNfParaCompra
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarTipoCalculoNfParaCompra); }
        }

        /// <summary>
        /// Define se ao finalizar a compra o preço de custo vai ser atualizado
        /// E caso haja markup definido os valores de venda tambem
        /// </summary>
        public static bool AtualizarValorProdutoFinalizarCompraComBaseMarkUp
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AtualizarValorProdutoFinalizarCompraComBaseMarkUp); }
        }

        /// <summary>
        /// Define se a decrição do beneficiamento será exibida de forma customizada
        /// </summary>
        public static bool ExibicaoDescrBenefCustomizada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibicaoDescrBenefCustomizada); }
        }
    }
}
