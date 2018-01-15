namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Armazena os detalhes da baixa do estoque.
    /// </summary>
    public class DetalhesBaixaEstoque
    {
        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd { get; set; }

        /// <summary>
        /// Identificador do produto no pedido
        /// </summary>
        public int IdProdPed { get; set; }

        /// <summary>
        /// Identificador do produto da baixa.
        /// </summary>
        public int IdProdBaixa { get; set; }

        /// <summary>
        /// Código interno do produto da baixa.
        /// </summary>
        public string CodInternoBaixa { get; set; }

        /// <summary>
        /// Descriação do produto da baixa.
        /// </summary>
        public string DescricaoBaixa { get; set; }

        /// <summary>
        /// Quantidade de baixa.
        /// </summary>
        public float Qtde { get; set; }

        #endregion
    }
}
