Vue.component('campo-desconto-quantidade', {
  props: {
    /**
     * Indica se o campo será exibido ou não na tela.
     * @type {?boolean}
     */
    exibir: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Percentual de desconto por quantidade.
     * @type {?number}
     */
    percentual: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    },

    /**
     * Valor de desconto por quantidade.
     * @type {?number}
     */
    valor: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
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
    }
  },

  data: function() {
    return {
      percentualDescontoMaximo: 0,
      descontoTabela: 0,
      percentualDescontoValidar: 0
    };
  },

  computed: {
    /**
     * Propriedade computada que retorna o percentual normalizado e que
     * atualiza a propriedade em caso de alteração.
     * @type {string}
     */
    percentualAtual: {
      get: function() {
        return this.percentual || '';
      },
      set: function(valor) {
        if (valor !== this.percentual) {
          this.$emit('update:percentual', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor normalizado e que
     * atualiza a propriedade em caso de alteração.
     * @type {string}
     */
    valorAtual: {
      get: function() {
        return this.valor || '';
      },
      set: function(valor) {
        if (valor !== this.valor) {
          this.$emit('update:valor', valor);
        }
      }
    }
  },

  template: '#CampoDescontoQuantidade-template'
});
