namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados de um item da listagem de produtos.
    /// </summary>
    public class ProdutoListagemItem
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd { get; set; }

        /// <summary>
        /// Código interno do produto.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição do tipo do produto.
        /// </summary>
        public string TipoProduto { get; set; }

        /// <summary>
        /// Descrição do produto
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Custom de fabricação base.
        /// </summary>
        public decimal Custofabbase { get; set; }

        /// <summary>
        /// Custo de compra.
        /// </summary>
        public decimal CustoCompra { get; set; }

        /// <summary>
        /// Valor do produto no atacado.
        /// </summary>
        public decimal ValorAtacado { get; set; }

        /// <summary>
        /// Valor do produto no balcão.
        /// </summary>
        public decimal ValorBalcao { get; set; }

        /// <summary>
        /// Valor obra.
        /// </summary>
        public decimal ValorObra { get; set; }

        /// <summary>
        /// Valor reposição.
        /// </summary>
        public decimal ValorReposicao { get; set; }

        /// <summary>
        /// Valor mínimo.
        /// </summary>
        public decimal ValorMinimo { get; set; }

        #endregion
    }
}
