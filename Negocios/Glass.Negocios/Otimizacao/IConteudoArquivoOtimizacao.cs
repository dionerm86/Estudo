using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios
{
    /// <summary>
    /// Assinatura do conteúdo de um arquivo de otimização.
    /// </summary>
    public interface IConteudoArquivoOtimizacao
    {
        #region Propriedades

        /// <summary>
        /// Nome do arquivo.
        /// </summary>
        string Nome { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Abre o conteúdo do arquivo.
        /// </summary>
        /// <returns></returns>
        System.IO.Stream Abrir();

        #endregion
    }
}
