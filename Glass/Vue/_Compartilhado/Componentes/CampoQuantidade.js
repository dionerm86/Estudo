Vue.component('campo-quantidade', {
  props: {
    /**
     * Quantidade selecionada no controle.
     * @type {number}
     */
    quantidade: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Percentual de desconto por quantidade selecionado no controle.
     * @type {?number}
     */
    percentualDescontoPorQuantidade: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Valor de desconto por quantidade selecionado no controle.
     * @type {?number}
     */
    valorDescontoPorQuantidade: {
      required: false,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se a quantidade pode ser decimal ou não.
     * @type {?boolean}
     */
    permiteDecimal: {
      required: false,
      twoWay: false,
      default: false,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * ID do produto para busca do desconto por quantidade.
     * @type {?number}
     */
    idProduto: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * ID do cliente para busca do desconto por quantidade.
     * @type {?number}
     */
    idCliente: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Indica se o campo de desconto por quantidade deve ser exibido ou não.
     * Se estiver marcado como 'true', o controle de quantidade será exibido de acordo com
     * seu controle interno.
     * @type {?boolean}
     */
    exibirDescontoPorQuantidade: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBoolean
    }
  },

  data: function() {
    return {
      exibirDescontoPorQuantidade_: false
    };
  },

  computed: {
    /**
     * Propriedade computada que retorna a quantidade normalizada, e que
     * atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    quantidadeAtual: {
      get: function() {
        return this.quantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.quantidade) {
          this.$emit('update:quantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o percentual de desconto por quantidade normalizado
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    percentualDescontoPorQuantidadeAtual: {
      get: function() {
        return this.percentualDescontoPorQuantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.percentualDescontoPorQuantidade) {
          this.$emit('update:percentualDescontoPorQuantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor de desconto por quantidade normalizado
     * e que atualiza a propriedade em caso de alteração.
     * @type {number}
     */
    valorDescontoPorQuantidadeAtual: {
      get: function() {
        return this.valorDescontoPorQuantidade || 0;
      },
      set: function(valor) {
        if (valor !== this.valorDescontoPorQuantidade) {
          this.$emit('update:valorDescontoPorQuantidade', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor do incremento do campo quantidade.
     * @type {number}
     */
    incremento: function() {
      return this.permiteDecimal ? 0.01 : 1;
    }
  },

  template: '#CampoQuantidade-template'
});
