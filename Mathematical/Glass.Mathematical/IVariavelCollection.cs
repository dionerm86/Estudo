using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Mathematical
{
    /// <summary>
    /// Assinatura de uma coleção de variáveis.
    /// </summary>
    public interface IVariavelCollection : IEnumerable<IVariavel>
    {
        #region Propriedades

        /// <summary>
        /// Recupera a variável na posição informada.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IVariavel this[int index] { get;}

        /// <summary>
        /// Recupera o valor da variável com base no nome informado.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        double this[string nome] { get; }

        /// <summary>
        /// Quantidade de variáveis na coleção.
        /// </summary>
        int Count { get; }

        #endregion

        #region Métodos

        /// <summary>
        /// Adiciona a variável para a coleção.
        /// </summary>
        /// <param name="variavel">Instancia da variável que será adicionada.</param>
        void Add(IVariavel variavel);

        /// <summary>
        /// Remove a variável da coleção.
        /// </summary>
        /// <param name="variavel">Instancia da variável que será removida.</param>
        /// <returns></returns>
        bool Remove(IVariavel variavel);

        /// <summary>
        /// Verifica se contém na coleção uma variável com o nome informado.
        /// </summary>
        /// <param name="nome">Nome da variável que será verificada.</param>
        /// <returns></returns>
        bool Contains(string nome);

        #endregion
    }
}
