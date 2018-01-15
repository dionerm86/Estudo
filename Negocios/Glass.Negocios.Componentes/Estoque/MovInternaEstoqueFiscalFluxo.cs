using Colosoft;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Estoque.Negocios.Componentes
{
    public class MovInternaEstoqueFiscalFluxo: IMovInternaEstoqueFiscalFluxo
    {
        /// <summary>
        /// Pesquisa as movimentações
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.MovInternaEstoqueFiscal> PesquisarMovimentacoes()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.MovInternaEstoqueFiscal>()
                .OrderBy("IdMovInternaEstoqueFiscal")
                .ToVirtualResultLazy<Entidades.MovInternaEstoqueFiscal>();
        }

        /// <summary>
        /// Recupera os dados da movimentação.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Entidades.MovInternaEstoqueFiscal ObtemProduto(int idProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.MovInternaEstoqueFiscal>()
                .Where("IdMovInternaEstoqueFiscal = ?id")
                .Add("?id", idProd)
                .ProcessLazyResult<Entidades.MovInternaEstoqueFiscal>()
                .FirstOrDefault();
        }
    }
}
