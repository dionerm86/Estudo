using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colosoft;

namespace Glass.Mathematical
{
    /// <summary>
    /// Representa uma coleção de variáveis.
    /// </summary>
    public class VariavelCollection : IVariavelCollection
    {
        #region Local Variables

        private readonly List<IVariavel> _variaveis;

        #endregion

        #region Propriedades

        /// <summary>
        /// Quantidade de itens na coleção.
        /// </summary>
        public int Count => _variaveis.Count;

        /// <summary>
        /// Recupera a define a variável pelo indice informado.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IVariavel this[int index]
        {
            get { return _variaveis[index]; }
        }

        /// <summary>
        /// Recupera o valor da variável pelo nome informado.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public double this[string nome]
        {
            get
            {
                for (var i = 0; i < _variaveis.Count; i++)
                    if (_variaveis[i].Nome == nome)
                        return _variaveis[i].ObterValor();

                return 0.0;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public VariavelCollection()
        {
            _variaveis = new List<IVariavel>();
        }

        /// <summary>
        /// Cria a instancia com a relação da variáveis informadas.
        /// </summary>
        /// <param name="variaveis"></param>
        public VariavelCollection(IEnumerable<IVariavel> variaveis)
        {
            variaveis.Require(nameof(variaveis)).NotNull();

            _variaveis = new List<IVariavel>(variaveis);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adiciona a variável para a coleção.
        /// </summary>
        /// <param name="variavel">Instancia da variável que será adicionada.</param>
        public void Add(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();

            if (Contains(variavel.Nome))
                throw new InvalidOperationException($"Já existem um variável com o nome '{variavel.Nome}' na coleção.");

            _variaveis.Add(variavel);
        }

        /// <summary>
        /// Remove a variável da coleção.
        /// </summary>
        /// <param name="variavel">Instancia da variável que será removida.</param>
        /// <returns></returns>
        public bool Remove(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();

            return _variaveis.Remove(variavel);
        }

        /// <summary>
        /// Verifica se contém na coleção uma variável com o nome informado.
        /// </summary>
        /// <param name="nome">Nome da variável que será verificada.</param>
        /// <returns></returns>
        public bool Contains(string nome)
        {
            return _variaveis.Any(f => f.Nome == nome);
        }

        #endregion

        #region Membros IEnumerable<IVariavel>

        /// <summary>
        /// Recupera o enumerador das variáveis.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IVariavel> GetEnumerator()
        {
            return _variaveis.GetEnumerator();
        }

        /// <summary>
        /// Recupera o enumerador das variáveis.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _variaveis.GetEnumerator();
        }

        #endregion
    }
}
