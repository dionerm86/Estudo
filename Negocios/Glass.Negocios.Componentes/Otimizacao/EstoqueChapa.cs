using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa o estoque de chapas.
    /// </summary>
    class EstoqueChapa : IEstoqueChapa
    {
        #region Propriedades

        /// <summary>
        /// Relação dos materiais do estoque.
        /// </summary>
        public IEnumerable<IMaterial> Materiais { get; }

        /// <summary>
        /// Entradas do estoque.
        /// </summary>
        public IEnumerable<IEntradaEstoqueChapa> Entradas { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="materiais"></param>
        /// <param name="entradas"></param>
        public EstoqueChapa(IEnumerable<IMaterial> materiais, IEnumerable<IEntradaEstoqueChapa> entradas)
        {
            Materiais = materiais;
            Entradas = entradas;
        }

        #endregion
    }
}
