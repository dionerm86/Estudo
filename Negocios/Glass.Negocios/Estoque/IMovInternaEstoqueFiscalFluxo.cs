using System.Collections.Generic;

namespace Glass.Estoque.Negocios
{
    public interface IMovInternaEstoqueFiscalFluxo
    {
        /// <summary>
        /// Pesquisa as movimentações
        /// </summary>
        /// <returns></returns>
        IList<Entidades.MovInternaEstoqueFiscal> PesquisarMovimentacoes();


        /// <summary>
        /// Recupera os dados da movimentação.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        Entidades.MovInternaEstoqueFiscal ObtemProduto(int idProd);
    }
}
