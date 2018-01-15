
namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor de produtos.
    /// </summary>
    public interface IProvedorProdutos
    {
        /// <summary>
        /// Recupera os dados do produto.
        /// </summary>
        Entidades.Produto ObtemProduto(int idProd);

        /// <summary>
        /// Recupera os descritores dos produtos
        /// </summary>
        System.Collections.Generic.IList<Colosoft.IEntityDescriptor> ObtemProdutos();
    }
}
