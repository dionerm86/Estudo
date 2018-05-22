using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa uma entrada do estoque de chapas.
    /// </summary>
    class EntradaEstoqueChapa : IEntradaEstoqueChapa
    {
        #region Propriedades

        /// <summary>
        /// Código do material.
        /// </summary>
        public string CodigoMaterial { get; set; }

        /// <summary>
        /// Posição da entrada.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Largura.
        /// </summary>
        public double Largura { get; set; }

        /// <summary>
        /// Altura.
        /// </summary>
        public double Altura { get; set; }

        /// <summary>
        /// Cavalete.
        /// </summary>
        public string Cavalete { get; set; }

        /// <summary>
        /// Prioridade.
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Tipo de corte suportado pela entrada.
        /// </summary>
        public TipoCorteTransversal TipoCorte { get; set; }

        /// <summary>
        /// Identifica se é uma chapa de retalho.
        /// </summary>
        public bool Retalho { get; set; }

        #endregion
    }
}
