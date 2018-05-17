using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura de uma peça padrão.
    /// </summary>
    public interface IPecaPadrao
    {
        #region Propriedades

        /// <summary>
        /// Código da peça.
        /// </summary>
        string Codigo { get; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        int Qtde { get; }

        /// <summary>
        /// Largura.
        /// </summary>
        double Largura { get; }

        /// <summary>
        /// Altura.
        /// </summary>
        double Altura { get; }

        /// <summary>
        /// Aresta.
        /// Representa uma cota adicional que deve adicionar-se às medidas da
        /// peça(lado X e Y), que serve para determinar as dimensões reais da peça
        /// para o corte e da peça acabada
        /// </summary>
        double Aresta { get; }

        /// <summary>
        /// Indica se é ou não possível rodar uma peça durante a fase de
        /// optimização, indispensável se o material utilizado apresentar um betado
        /// especial ou um desenho que obrigue à colocação das peças sempre na
        /// mesma direcção.Prejudica o resultado da optimização, já que limita as
        /// combinações possíveis da mesma.
        /// </summary>
        bool PodeRotacionar { get; }

        /// <summary>
        /// Aresta X1.
        /// É o valor de amoladura no lado X1 (lado inferior) em caso de
        /// amoladura nos 4 lados.
        /// </summary>
        double ArestaX1 { get;}

        /// <summary>
        /// Aresta Y1.
        /// É o valor de amoladura no lado Y1 (lado esquerdo) em caso de
        /// amoladura nos 4 lados.
        /// </summary>
        double ArestaY1 { get; }

        /// <summary>
        /// Aresta X2.
        /// É o valor de amoladura no lado X2 (lado superior) em caso de
        /// amoladura nos 4 lados.
        /// </summary>
        double ArestaX2 { get; set; }

        /// <summary>
        /// Aresta Y2.
        /// É o valor de amoladura no lado Y2 (lado direito) em caso de
        /// amoladura nos 4 lados.
        /// </summary>
        double ArestaY2 { get; set; }

        #endregion
    }
}
