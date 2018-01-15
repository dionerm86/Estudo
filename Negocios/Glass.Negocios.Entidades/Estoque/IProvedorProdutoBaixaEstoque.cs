namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Assinatura o provedor de produto baixa estoque.
    /// </summary>
    public interface IProvedorProdutoBaixaEstoque
    {
        /// <summary>
        /// Recupera a identificação do produto baixa estoque.
        /// </summary>
        /// <param name="idProdBaixa"></param>
        /// <param name="qtde"></param>
        /// <returns></returns>
        string ObtemIdentificacao(int idProdBaixa, float qtde);
    }
}
