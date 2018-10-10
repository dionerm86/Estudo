const app = new Vue({
  el: '#app',
  mixins: [Mixins.OrdenacaoLista('id', 'desc')],

  data: {
    filtro: {},
    configuracoes: {},
    agruparImpressao: null
  },

  methods: {
    /**
     * Recupera a lista de peças para a tela de consulta de produção.
     * @param {?Object} filtro Objeto com os filtros a serem usados para a busca de peças.
     * @param {number} pagina O número da página de resultados a ser exibida.
     * @param {number} numeroRegistros O número de registros que serão exibidos na página.
     * @param {string} ordenacao A ordenação para o resultado.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    buscarPecas: function (filtro, pagina, numeroRegistros, ordenacao) {
      if (!this.configuracoes || !Object.keys(this.configuracoes).length) {
        return Promise.reject();
      }

      return Servicos.Producao.obterLista(filtro, pagina, numeroRegistros, ordenacao);
    }
  },

  mounted: function () {
    var vm = this;

    Servicos.Producao.obterConfiguracoesConsulta()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
