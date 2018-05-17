using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa um material.
    /// </summary>
    class Material : IMaterial
    {
        #region Propriedades

        /// <summary>
        /// Código do material.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Tipo do material.
        /// </summary>
        public TipoMaterial Tipo { get; set; }

        /// <summary>
        /// Espessura 1.
        /// </summary>
        public double Espessura1 { get; set; }

        /// <summary>
        /// Espessura 2.
        /// </summary>
        public double Espessura2 { get; set; }

        /// <summary>
        /// Espessura 3.
        /// </summary>
        public double Espessura3 { get; set; }

        /// <summary>
        /// Espessura 4.
        /// </summary>
        public double Espessura4 { get; set; }

        /// <summary>
        /// Distancia mínima.
        /// </summary>
        public double DistanciaMinima { get; set; }

        /// <summary>
        /// Recorte X1.
        /// </summary>
        public double RecorteX1 { get; set; }

        /// <summary>
        /// Recorte Y1.
        /// </summary>
        public double RecorteY1 { get; set; }

        /// <summary>
        /// Recorte X2.
        /// </summary>
        public double RecorteX2 { get; set; }

        /// <summary>
        /// Aresta Y2.
        /// </summary>
        public double RecorteY2 { get; set; }

        /// <summary>
        /// Corte transversal em X.
        /// </summary>
        public double CorteTransversalX { get; set; }

        /// <summary>
        /// Corte transversal em Y.
        /// </summary>
        public double CorteTransversalY { get; set; }

        /// <summary>
        /// Valor mínimo em X para gerar um retalho.
        /// </summary>
        public double RetalhoMinX { get; set; }

        /// <summary>
        /// Valor mínimo em Y para gerar um retalho.
        /// </summary>
        public double RetalhoMinY { get; set; }

        /// <summary>
        /// Configura o valor de recorte que deve introduzir-se nas
        /// formas no caso de esta conter ângulos inferiores ao configurado no campo
        /// "Ângulo".
        /// </summary>
        public double RecorteAutomaticoForma { get; set; }

        /// <summary>
        /// Valor de ângulo ao qual o recorte deve ser introduzido de
        /// forma automática.
        /// </summary>
        public double AnguloRecorteAutomaticoForma { get; set; }

        #endregion
    }
}
