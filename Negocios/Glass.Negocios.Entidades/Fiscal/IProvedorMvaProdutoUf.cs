using Glass.Global.Negocios.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do provedor de MVA do produto por UF.
    /// </summary>
    public interface IProvedorMvaProdutoUf
    {
        /// <summary>
        /// Busca o valor do MVA por produto e UF dos produtos informados.
        /// </summary>
        /// <param name="produtos"></param>
        /// <param name="loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="cliente"></param>
        /// <param name="saida"></param>
        /// <returns></returns>
        IEnumerable<float> ObterMvaPorProdutos(
            IEnumerable<Produto> produtos, Loja loja, Fornecedor fornecedor, Cliente cliente, bool saida);
    }
}
