using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Assinatura do item de rentabilidade com referências.
    /// </summary>
    /// <typeparam name="TReferencia"></typeparam>
    interface IItemRentabilidadeComReferencias<out TReferencia> : IItemRentabilidade
    {
        #region Propriedades

        /// <summary>
        /// Referencias.
        /// </summary>
        IEnumerable<TReferencia> Referencias { get; }

        #endregion
    }
}
