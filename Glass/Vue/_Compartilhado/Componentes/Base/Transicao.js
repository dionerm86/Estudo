Vue.component('transicao', {
  props: {
    /**
     * Define se o componente será exibido.
     * @type {boolean}
     */
    exibir: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarBoolean
    },

    /**
     * Define se o componente será exibido com quebra de linha antes e depois.
     * @type {?boolean}
     */
    exibirComoBloco: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBoolean
    }
  },

  data: function () {
    return {
      estiloTransicao: {
        opacity: 0,
        transition: 'opacity 0.2s ease-in-out',
        padding: 0,
        margin: 0,
        border: 0
      }
    }
  },

  methods: {
    /**
     * Altera a opacidade do tooltip (para efeito de transição), exibindo-o.
     * @param {Object} elemento O elemento HTML que está sendo exibido.
     */
    entrar__: function (elemento) {
      this.$emit('entrar', elemento);
    },

    /**
     * Altera a opacidade do tooltip (para efeito de transição), exibindo-o.
     * @param {Object} elemento O elemento HTML que está sendo exibido.
     */
    mostrar__: function (elemento) {
      this.$emit('exibir', elemento);
      elemento.style.opacity = 1;
    },

    /**
     * Altera a opacidade do tooltip (para efeito de transição), escondendo-o.
     * @param {Object} elemento O elemento HTML que está sendo escondido.
     */
    esconder__: function (elemento) {
      this.$emit('esconder', elemento);
      elemento.style.opacity = 0;
    }
  },

  template: '#Transicao-template'
});
