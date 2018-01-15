using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    public interface IProvedorCartaoNaoIdentificado
    {
        /// <summary>
        /// Recupera a referencia do cartão não identificado.
        /// </summary>
        /// <param name="contaReceber"></param>
        /// <returns></returns>
        string ObterReferencia(ICartaoNaoIdentificado cartaoNaoIdentificado);

        /// <summary>
        /// Recupera se o valor do cartão pode ser editado
        /// </summary>
        bool EditarValor(int idCartaoNaoIdentificado);
    }
}
