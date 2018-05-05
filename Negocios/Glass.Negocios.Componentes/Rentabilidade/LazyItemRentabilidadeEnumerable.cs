using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade.Negocios.Componentes
{
    /// <summary>
    /// Implementação de um enumerador para os itens de rentabilidade com carga tardia.
    /// </summary>
    sealed class LazyItemRentabilidadeEnumerable : IEnumerable<IItemRentabilidade>
    {
        #region Variáveis Locais

        private IEnumerable<IItemRentabilidade> _fonte;
        private List<IItemRentabilidade> _lista;

        #endregion

        #region Propriedades

        /// <summary>
        /// Lista associada.
        /// </summary>
        private List<IItemRentabilidade> Lista
        {
            get
            {
                if (_lista == null)
                {
                    _lista = _fonte.ToList();
                    _fonte = null;
                }

                return _lista;
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fonte"></param>
        public LazyItemRentabilidadeEnumerable(IEnumerable<IItemRentabilidade> fonte)
        {
            _fonte = fonte;
        }

        #endregion

        #region Métodos Públicos

        public IEnumerator<IItemRentabilidade> GetEnumerator()
        {
            return new LazyItemRentabilidadeEnumerator(new Lazy<IEnumerator<IItemRentabilidade>>(() => Lista.GetEnumerator()));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Implementação do enumerador para itens de rentabilidade com carga tardia.
    /// </summary>
    sealed class LazyItemRentabilidadeEnumerator : IEnumerator<IItemRentabilidade>
    {
        #region Variáveis Locais

        private readonly Lazy<IEnumerator<IItemRentabilidade>> _lazy;

        #endregion

        #region Propriedaes

        public IItemRentabilidade Current => _lazy.Value.Current;

        object IEnumerator.Current => _lazy.Value.Current;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="fonte"></param>
        public LazyItemRentabilidadeEnumerator(Lazy<IEnumerator<IItemRentabilidade>> lazy)
        {
            _lazy = lazy;
        }

        #endregion

        #region Métodos Públicos

        public void Dispose() => _lazy.Value.Dispose();

        public bool MoveNext() => _lazy.Value.MoveNext();

        public void Reset() => _lazy.Value.Dispose();

        #endregion
    }
}
