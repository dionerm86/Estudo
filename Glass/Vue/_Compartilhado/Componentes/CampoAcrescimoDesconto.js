Vue.component('campo-acrescimo-desconto', {
  props: {
    /**
     * Tipo de acréscimo/desconto (percentual [1] ou valor [2]).
     * @type {number}
     */
    tipo: {
      required: true,
      validator: Mixins.Validacao.validarValoresOuVazio(1, 2)
    },

    /**
     * O valor (ou percentual, de acordo com o tipo) do acréscimo/desconto.
     * @type {number}
     */
    valor: {
      required: true,
      validator: Mixins.Validacao.validarNumeroOuVazio
    }
  },

  data: function() {
    return {
      unidadeMonetaria: ''
    };
  },

  created: function() {
    this.unidadeMonetaria = Formatacao.unidadeMonetaria();
  },

  computed: {
    /**
     * Propriedade computada que retorna o tipo normalizado, e que
     * atualiza a propriedade quando é alterada.
     * @type {number}
     */
    tipoAtual: {
      get: function() {
        return this.tipo || 2;
      },
      set: function(valor) {
        if (valor !== this.tipo) {
          this.$emit('update:tipo', valor);
        }
      }
    },

    /**
     * Propriedade computada que retorna o valor normalizado, e que
     * atualiza a propriedade quando é alterada.
     * @type {number}
     */
    valorAtual: {
      get: function() {
        return this.valor || 0;
      },
      set: function(valor) {
        valor = valor ? parseFloat(valor) : 0;

        if (valor !== this.valor) {
          this.$emit('update:valor', valor);
        }
      }
    }
  },

  template: '#CampoAcrescimoDesconto-template'
});
