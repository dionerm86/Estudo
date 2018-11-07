Vue.component('integradores-itemlogger', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do item do logger.
     * @type {Object}
     **/
    item: {
      required: true,
      twoWay: false
    }
  },

  data: function () {
    return {
      podeExibirErro: false
    }
  },

  methods: {
    /**
     * Altera a exibição dos dados do erro.
     **/
    alterarExibicaoErro: function () {
      if (this.item.categoria == 'Exception') {
        this.podeExibirErro = !this.podeExibirErro;
      }
    }
  },

  template: '#LstIntegradores-ItemLogger-template'
});
