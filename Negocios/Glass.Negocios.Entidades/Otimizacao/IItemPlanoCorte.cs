using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Entidades
{
    /// <summary>
    /// Assinatura de um item do plano de corte.
    /// </summary>
    public interface IItemPlanoCorte
    {
        #region Propriedades

        /// <summary>
        /// Obtém a posição do item.
        /// </summary>
        int Posicao { get; }

        #endregion
    }
}
