using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Representa um registro do estoque de capas.
    /// </summary>
    public class EstoqueChapa
    {
        #region Propriedades

        /// <summary>
        /// Código do material da chapa.
        /// </summary>
        public string CodMaterial { get; set; }

        /// <summary>
        /// Altura.
        /// </summary>
        public int Altura { get; set; }

        /// <summary>
        /// Largura.
        /// </summary>
        public int Largura { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtde { get; set; }

        /// <summary>
        /// Identifica se é uma chapa de retalho.
        /// </summary>
        public bool Retalho { get; set; }

        /// <summary>
        /// Posição da chapa.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Prioridade da chapa.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Tipo de corte transversal.
        /// </summary>
        public string CrosscutType { get; set; }

        #endregion
    }
}
