using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical
{
    /// <summary>
    /// Assinatura de um provedor de dependencias.
    /// </summary>
    interface IProvedorDependencias
    {
        #region Propriedades

        /// <summary>
        /// Nome das variáveis associadas.
        /// </summary>
        IEnumerable<string> Variaveis { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Adiciona uma variável de dependência.
        /// </summary>
        /// <param name="nome"></param>
        void AdicionarDependencia(string nome);

        #endregion
    }
}
