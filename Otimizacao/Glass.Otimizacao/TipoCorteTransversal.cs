using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Possíveis tipos de cortes transversais.
    /// </summary>
    public enum TipoCorteTransversal
    {
        /// <summary>
        /// Identifica que a transversal pode ser gerada do lado curto da chapa.
        /// </summary>
        X = 1,
        /// <summary>
        /// Identifica que a transversal pode ser gerada do lado comprido da chapa.
        /// </summary>
        Y,
        /// <summary>
        /// Identifica que o programa de otimização vai escolher o melhor lado.
        /// </summary>
        XY
    }
}
