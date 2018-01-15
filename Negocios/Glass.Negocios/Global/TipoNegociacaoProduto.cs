using System.ComponentModel;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Possíveis tipos de negociação do produto.
    /// </summary>
    public enum TipoNegociacaoProduto
    {
        /// <summary>
        /// Venda.
        /// </summary>
        [Description("Venda")]
        Venda = 1,
        /// <summary>
        /// Compra.
        /// </summary>
        [Description("Compra")]
        Compra,
        /// <summary>
        /// Compra e Venda
        /// </summary>
        [Description("Compra e venda")]
        CompraVenda
    }
}
