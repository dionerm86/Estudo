namespace Glass.Data.Helper.Estoque.Estrategia
{
    /// <summary>
    /// Cenário da baixa/crédito do estoque.
    /// </summary>
    public enum Cenario
    {
        /// <summary>
        /// Confirmação de pedido
        /// </summary>
        ConfirmacaoPedido,

        /// <summary>
        /// Cancelamento de pedido
        /// </summary>
        CancelamentoPedido,

        /// <summary>
        /// Liberação de pedido
        /// </summary>
        LiberacaoPedido,

        /// <summary>
        /// Cancelamento de liberação de pedido
        /// </summary>
        CancelamentoLiberacaoPedido,

        /// <summary>
        /// Troca/Devolução
        /// </summary>
        TrocaDevolucao,

        /// <summary>
        /// Cancelamento Troca/Devolução
        /// </summary>
        CancelamentoTrocaDevolucao,
    }
}