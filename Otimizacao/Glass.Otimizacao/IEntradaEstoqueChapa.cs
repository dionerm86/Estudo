using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Representa um entrada do estoque de chapas.
    /// </summary>
    public interface IEntradaEstoqueChapa
    {
        #region Propriedades

        /// <summary>
        /// Código do material.
        /// </summary>
        string CodigoMaterial { get; }

        /// <summary>
        /// Posição da entrada.
        /// </summary>
        int Posicao { get; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        int Quantidade { get; }

        /// <summary>
        /// Largura.
        /// </summary>
        double Largura { get; }

        /// <summary>
        /// Altura.
        /// </summary>
        double Altura { get; }

        /// <summary>
        /// Cavalete.
        /// </summary>
        string Cavalete { get; }

        /// <summary>
        /// Prioridade.
        /// </summary>
        int Prioridade { get; }

        /// <summary>
        /// Tipo de corte suportado pela entrada.
        /// </summary>
        TipoCorteTransversal TipoCorte { get; }

        /// <summary>
        /// Identifica se é uma chapa de retalho.
        /// </summary>
        bool Retalho { get; }

        #endregion
    }
}
