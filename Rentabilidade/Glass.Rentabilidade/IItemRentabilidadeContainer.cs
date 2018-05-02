using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Assinatura do container de itens de rentabilidade.
    /// </summary>
    public interface IItemRentabilidadeContainer
    {
        /// <summary>
        /// Itens filhos.
        /// </summary>
        IEnumerable<IItemRentabilidade> Itens { get; }
    }
}
