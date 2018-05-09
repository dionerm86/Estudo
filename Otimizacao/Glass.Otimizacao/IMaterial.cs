using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Possíveis tipos de materiais.
    /// </summary>
    public enum TipoMaterial
    {
        /// <summary>
        /// Representa o vidro com uma camada.
        /// </summary>
        Monolitico = 1,
        /// <summary>
        /// Laminado
        /// </summary>
        Laminado
    }

    /// <summary>
    /// Assinatura do material que será considerado na otimização.
    /// </summary>
    public interface IMaterial
    {
        #region Propriedades

        /// <summary>
        /// Código do material.
        /// </summary>
        string Codigo { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Tipo do material.
        /// </summary>
        TipoMaterial Tipo { get; }

        /// <summary>
        /// Espessura 1.
        /// </summary>
        double Espessura1 { get; }

        /// <summary>
        /// Espessura 2.
        /// </summary>
        double Espessura2 { get; }

        /// <summary>
        /// Espessura 3.
        /// </summary>
        double Espessura3 { get; }

        /// <summary>
        /// Espessura 4.
        /// </summary>
        double Espessura4 { get; }

        /// <summary>
        /// Distancia mínima.
        /// </summary>
        double DistanciaMinima { get; }

        /// <summary>
        /// Recorte X1.
        /// </summary>
        double RecorteX1 { get; }

        /// <summary>
        /// Recorte Y1.
        /// </summary>
        double RecorteY1 { get; }

        /// <summary>
        /// Recorte X2.
        /// </summary>
        double RecorteX2 { get; }

        /// <summary>
        /// Aresta Y2.
        /// </summary>
        double RecorteY2 { get; }

        /// <summary>
        /// Corte transversal em X.
        /// </summary>
        double CorteTransversalX { get; }

        /// <summary>
        /// Corte transversal em Y.
        /// </summary>
        double CorteTransversalY { get; }

        /// <summary>
        /// Valor mínimo em X para gerar um retalho.
        /// </summary>
        double RetalhoMinX { get; }

        /// <summary>
        /// Valor mínimo em Y para gerar um retalho.
        /// </summary>
        double RetalhoMinY { get; }

        /// <summary>
        /// Configura o valor de recorte que deve introduzir-se nas
        /// formas no caso de esta conter ângulos inferiores ao configurado no campo
        /// "Ângulo".
        /// </summary>
        double RecorteAutomaticoForma { get; }

        /// <summary>
        /// Valor de ângulo ao qual o recorte deve ser introduzido de
        /// forma automática.
        /// </summary>
        double AnguloRecorteAutomaticoForma { get; }

        #endregion
    }
}
