Vue.component('campo-largura', {
  inheritAttrs: false,
  props: {
    /**
     * Largura que é exibida no controle.
     * @type {?number}
     */
    largura: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se a largura pode ser decimal ou não.
     * @type {?boolean}
     */
    permiteDecimal: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se a largura pode ser editada ou não.
     * @type {?boolean}
     */
    permiteEditar: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o valor normalizado da largura, e que
     * atualiza o valor da propriedade em caso de alteração.
     * @type {number}
     */
    larguraAtual: {
      get: function() {
        return this.largura || 0;
      },
      set: function(valor) {
        if (valor !== this.largura) {
          this.$emit('update:largura', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor do incremento do campo largura.
     * @type {number}
     */
    incremento: function() {
      return this.permiteDecimal ? 0.01 : 1;
    }
  },

  template: '#CampoLargura-template'
});
