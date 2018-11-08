using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Enumerador com os tipos que podem ser definidos na configuração ComissaoPorContasRecebidas
    /// </summary>
    public enum TipoComissaoContaRec
    {
        /// <summary>
        /// Controle está desabilitado
        /// </summary>
        Desabilitado,

        /// <summary>
        /// Deverá ser usado o vendedor associado ao Pedido no momento da geração da conta.
        /// </summary>
        [Description("Vendedor associado ao Pedido.")]
        VendedorAssociadoPedido,

        /// <summary>
        /// Deverá ser usado o vendedor associado ao Cliente no momento da geração da conta.
        /// </summary>
        [Description("Vendedor associado ao Cliente.")]
        VendedorAssociadoCliente,
    }
}
