using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa uma sessão de otimização.
    /// </summary>
    class SessaoOtimizacao : ISessaoOtimizacao
    {
        #region Variáveis Locais

        private readonly IEstoqueChapa _estoqueChapas;
        private readonly IEnumerable<IPecaPadrao> _pecasPadrao;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="estoqueChapa">Estoque de chapas.</param>
        /// <param name="pecasPadrao">Relação das peças padrão.</param>
        public SessaoOtimizacao(IEstoqueChapa estoqueChapa, IEnumerable<IPecaPadrao> pecasPadrao)
        {
            _estoqueChapas = estoqueChapa;
            _pecasPadrao = pecasPadrao;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o estoque de chapas.
        /// </summary>
        /// <returns></returns>
        public IEstoqueChapa ObterEstoqueChapas() => _estoqueChapas;

        /// <summary>
        /// Recupera as peças padrão.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPecaPadrao> ObterPecasPadrao() => _pecasPadrao;

        #endregion
    }
}
