using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao
{
    /// <summary>
    /// Assinatura do arquivo da solução de otimização.
    /// </summary>
    public interface IArquivoSolucaoOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Obtém o nome do arquivo.
        /// </summary>
        string Nome { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Abrea a leitura do arquivo.
        /// </summary>
        /// <returns></returns>
        System.IO.Stream Abrir();

        #endregion
    }
}
