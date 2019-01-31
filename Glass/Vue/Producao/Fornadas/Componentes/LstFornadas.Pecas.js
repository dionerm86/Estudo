Vue.component('fornadas-pecas', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de peças de fornada.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  methods: {
    /**
     * Obtém a lista de peças de fornada.
     * @returns {Promise} Uma promise com a busca dos itens, de acordo com o filtro.
     */
    obterLista: function () {
      return Servicos.Producao.Fornadas.obterPecasFornada(this.filtro.idFornada);
    }
  },

  template: '#LstFornadas-Pecas-template'
});
