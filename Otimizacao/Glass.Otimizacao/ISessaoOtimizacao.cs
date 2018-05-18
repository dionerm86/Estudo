using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura do provedor da otimização.
    /// </summary>
    public interface ISessaoOtimizacao
    {
        #region Métodos

        /// <summary>
        /// Recupera o estoque de chapas.
        /// </summary>
        /// <returns></returns>
        IEstoqueChapa ObterEstoqueChapas();

        /// <summary>
        /// Recupera as peças padrão.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPecaPadrao> ObterPecasPadrao();

        #endregion
    }
}
