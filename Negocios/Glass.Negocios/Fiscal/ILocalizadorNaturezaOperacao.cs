using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do localizador de natureza de operação.
    /// </summary>
    public interface ILocalizadorNaturezaOperacao
    {
        /// <summary>
        /// Localiza as naturezas de operação para os produtos informados,
        /// com base no cliente e loja.
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="produtos"></param>
        /// <returns>Relação da natureza de operação para cada produto.</returns>
        IEnumerable<Entidades.NaturezaOperacao> Buscar(
            Global.Negocios.Entidades.Cliente cliente, Global.Negocios.Entidades.Loja loja,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos);
    }
}
