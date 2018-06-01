using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data
{
    /// <summary>
    /// Assinatura do resultado do calculo do imposto.
    /// </summary>
    public interface ICalculoImpostoResultado
    {
        #region Métodos

        /// <summary>
        /// Salva os dados do resultado na sessão informada.
        /// </summary>
        /// <param name="sessao"></param>
        void Salvar(GDA.GDASession sessao);

        #endregion
    }
}
