using System.Collections.Generic;

namespace Glass.Estoque.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do repositório das baixas do estoque do produto.
    /// </summary>
    public interface IProdutoBaixaEstoqueRepositorio
    {
        /// <summary>
        /// Obtem os detalhes das baixas do estoque do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        IEnumerable<DetalhesBaixaEstoque> ObtemDetalhesBaixasEstoque(int idProd);
    }
}
