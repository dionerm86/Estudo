const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('codInterno', 'asc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca a lista de chapas disponíveis.
     * @param {Object} filtro O filtro definido na tela de pesquisa.
     * @param {number} pagina O número da página que está sendo buscada.
     * @param {number} numeroRegistros O número de registros que serão retornados.
     * @param {string} ordenacao A ordenação que será aplicada ao resultado.
     * @return {Promise} Uma promise com o resultado da busca.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      return Servicos.ChapasDisponiveis.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    },

    /**
       * Recupera o objeto com as .
       * @returns {Promise} Uma promise com o resultado da busca.
       */
    obterItensFiltroCor: function () {
      return Servicos.CoresVidro.obterParaControle;
    },

    /**
     * Retorna os itens para o controle de motoristas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroLoja: function () {
      return Servicos.Lojas.obterParaControle();
    }
  }
});
