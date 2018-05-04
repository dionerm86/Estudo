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
    /// Representa um agregador de coleções de variáveis.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    public class AggregateVariavelCollection : IVariavelCollection
    {
        #region Variáveis Locais

        private readonly List<IVariavelCollection> _collections = new List<IVariavelCollection>();
        private readonly IVariavelCollection _variaveis = new VariavelCollection();

        #endregion

        #region Propriedades

        /// <summary>
        /// Quantidade de variáveis na coleção.
        /// </summary>
        public int Count => _collections.Count > 0 ? _collections.Sum(f => f.Count) : 0;

        /// <summary>
        /// Recupera a variável pelo indice informado.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IVariavel this[int index]
        {
            get
            {
                var maxIndex = 0;

                foreach (var i in _collections)
                {
                    // Verifica o indice está na coleção informada
                    if (index <= (maxIndex + (i.Count - 1)))
                    {
                        return i[index - maxIndex];
                    }

                    maxIndex += i.Count;

                }

                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        /// <summary>
        /// Recupera o valor da variável com base no nome informado.
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public double this[string nome]
        {
            get
            {
                foreach (var i in _collections)
                    if (i.Contains(nome))
                        return i[nome];

                throw new ArgumentOutOfRangeException(nameof(nome));
            }
        }

        /// <summary>
        /// Quantidade de coleção no agregador.
        /// </summary>
        public int CollectionsCount => _collections.Count;

        /// <summary>
        /// Coleções do agregador.
        /// </summary>
        public IEnumerable<IVariavelCollection> Collections => _collections.Select(f => f);

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public AggregateVariavelCollection()
        {
            _collections.Add(_variaveis);
        }

        /// <summary>
        /// Cria a instancia com as relação de coleções informada.
        /// </summary>
        /// <param name="collections"></param>
        public AggregateVariavelCollection(IEnumerable<IVariavelCollection> collections)
            : base()
        {
            if (collections != null)
                _collections.AddRange(collections);
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Adiciona a variável para a coleção.
        /// </summary>
        /// <param name="variavel"></param>
        public void Add(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();

            if (Contains(variavel.Nome))
                throw new InvalidOperationException($"Já existem um variável com o nome '{variavel.Nome}' na coleção.");

            _variaveis.Add(variavel);
        }

        /// <summary>
        /// Remove a variável informada da coleção.
        /// </summary>
        /// <param name="variavel"></param>
        /// <returns></returns>
        public bool Remove(IVariavel variavel)
        {
            variavel.Require(nameof(variavel)).NotNull();

            foreach (var collection in _collections)
                if (collection.Remove(variavel))
                    return true;

            return false;
        }

        /// <summary>
        /// Adiciona uma coleção para o agregador.
        /// </summary>
        /// <param name="collection"></param>
        public void Add(IVariavelCollection collection)
        {
            _collections.Add(collection);
        }

        /// <summary>
        /// Remove a coleção do gregador.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public bool Remove(IVariavelCollection collection)
        {
            return _collections.Remove(collection);
        }

        /// <summary>
        /// Verifica se contém na coleção uma variável com o nome informado.
        /// </summary>
        /// <param name="nome">Nome da variável que será verificada.</param>
        /// <returns></returns>
        public bool Contains(string nome)
        {
            return _collections.Any(f => f.Contains(nome));
        }

        #endregion

        #region Membros IEnumerable<IVariavel>

        /// <summary>
        /// Recupera o enumerador das variáveis.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IVariavel> GetEnumerator()
        {
            foreach (var collection in _collections)
                foreach (var i in collection)
                    yield return i;
        }

        /// <summary>
        /// Recupera o enumerador das variáveis.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
