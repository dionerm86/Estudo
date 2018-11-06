Vue.component('integradores-itemhistorico', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Dados do item do esquema do histórico.
     * @type {Object}
     */
    itemEsquema: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Item do histórico.
     * @type {Object}
     **/
    item: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      exibirFalha: false,
    }
  },

  methods: {
    alterarExibicaoFalha: function () {
      if (this.item.falha != null) {
        this.exibirFalha = !this.exibirFalha;
      }
    },
  },

  template: '#LstIntegradores-ItemHistorico-template'
});
