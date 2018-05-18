using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura do estoque de chapas.
    /// </summary>
    public interface IEstoqueChapa
    {
        #region Propriedades

        /// <summary>
        /// Relação dos materiais do estoque.
        /// </summary>
        IEnumerable<IMaterial> Materiais { get; }

        /// <summary>
        /// Entradas do estoque.
        /// </summary>
        IEnumerable<IEntradaEstoqueChapa> Entradas { get; }

        #endregion
    }
}
