using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios
{
    /// <summary>
    /// Assinatura do provedor de item de rentabilidade para o tipo informado.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProvedorItemRentabilidade<in T>
    {
        /// <summary>
        /// Recupera o item da rentabilidade com base na referencia informada.
        /// </summary>
        /// <param name="referencia"></param>
        /// <returns></returns>
        IItemRentabilidade ObterItem(T referencia);

        /// <summary>
        /// Recupera o item da rentabilidade com base no identificador associado com o tipo do provedor.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IItemRentabilidade ObterItem(int id);
    }
}
