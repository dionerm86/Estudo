using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um plano de corte.
    /// </summary>
    class PlanoCorte
    {
        #region Propriedades

        /// <summary>
        /// Obtém ou define o identificador do plano de corte.
        /// </summary>
        public int IdPlanoCorte { get; set; }

        /// <summary>
        /// Obtém ou define a posição.
        /// </summary>
        public int Posicao { get; set; }

        #endregion
    }
}
