namespace Glass.Data
{
    /// <summary>
    /// Possíveis tipos de beneficiamento de produto.
    /// </summary>
    public enum TipoProdutoBeneficiamento
    {
        /// <summary>
        /// Nenhum.
        /// </summary>
        Nenhum,
        /// <summary>
        /// Material do projeto.
        /// </summary>
        MaterialProjeto,
        /// <summary>
        /// Produto da compra.
        /// </summary>
        ProdutoCompra,
        /// <summary>
        /// Produto do orçamento.
        /// </summary>
        ProdutoOrcamento,
        /// <summary>
        /// Produto do pedido.
        /// </summary>
        ProdutoPedido,
        /// <summary>
        /// Produto do pedido espelho.
        /// </summary>
        ProdutoPedidoEspelho,
        /// <summary>
        /// Produto.
        /// </summary>
        Produto,
        /// <summary>
        /// Produto da troca devolução.
        /// </summary>
        ProdutoTrocaDevolucao,
        /// <summary>
        /// Peça do modelo do projeto.
        /// </summary>
        PecaModeloProjeto,
        /// <summary>
        /// Peça do item do projeto.
        /// </summary>
        PecaItemProjeto,
        /// <summary>
        /// Produto trocado.
        /// </summary>
        ProdutoTrocado,
        /// <summary>
        /// Produto baixa estoque
        /// </summary>
        ProdutoBaixaEst
    }
}
