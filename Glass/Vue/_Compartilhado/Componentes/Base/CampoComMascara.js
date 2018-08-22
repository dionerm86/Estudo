Vue.component('masked-input', vueTextMask.default);

Vue.component('campo-com-mascara', {
  props: {
    /**
     * Valor selecionado no controle.
     * @type {string}
     */
    valor: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarStringOuVazio
    },

    /**
     * Máscara a ser exibida no controle.
     * @type {string}
     */
    mascara: {
      type: [Array, Function, Boolean, Object],
      required: true,
      twoWay: false
    },
  },

  methods: {
    /**
     * Método executado pelo controle 'masked-input' com o valor selecionado no controle.
     * Atualiza a propriedade 'valor' com o texto do controle.
     * @param {string} valor O valor que está no controle.
     */
    atualizaValor: function (valor) {
      if (valor !== this.valor) {
        this.$emit('update:valor', valor);
      }
    }
  },

  template: '#CampoComMascara-template'
});
