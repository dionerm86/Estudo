Vue.component('campo-beneficiamento-selecao-multipla-inclusiva', {
  inheritAttrs: false,
  mixins: [Mixins.Comparar, Mixins.UUID, Mixins.CampoBeneficiamento],
  props: {
    /**
     * Beneficiamento que o controle representa.
     * @type {!Object}
     */
    beneficiamento: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Itens selecionados no controle.
     * @type {?Object[]}
     */
    itensSelecionados: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarArrayOuVazio
    }
  },

  data: function () {
    return {
      uuid: null,
      selecionados: (this.itensSelecionados || []).map(function (item) {
        return item.id
      })
    };
  },

  methods: {
    /**
     * Recupera um identificador único para o beneficiamento.
     * @param {!Object} beneficiamento O beneficiamento que está sendo criado.
     * @returns {string} Uma string com o identificador do beneficiamento.
     */
    idUnico: function (beneficiamento) {
      return this.uuid + '_' + beneficiamento.id;
    }
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamento'.
     * Reinicia os valores selecionados no controle se o beneficiamento for alterado.
     */
    beneficiamento: {
      handler: function () {
        this.selecionados = [];
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionados'.
     * Atualiza os itens selecionados internamente com os novos valores.
     */
    itensSelecionados: {
      handler: function (atual) {
        var selecionados = !atual || !atual.length
          ? []
          : atual.map(function (item) {
            return item.id;
          });

        if (!this.equivalentes(selecionados, this.selecionados)) {
          this.selecionados = selecionados;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'selecionados'.
     * Altera os itens selecionados com os valores escolhidos no controle.
     */
    selecionados: {
      handler: function (atual) {
        if (!atual || !atual.length) {
          this.$emit('update:itensSelecionados', null);
        } else {
          var itensSelecionados = this.beneficiamento.filhos
            .filter(function (item) {
              return atual.indexOf(item.id) > -1;
            })
            .map(function (item) {
              return this.criarBeneficiamento({
                id: item.id
              });
            });

          if (!this.equivalentes(itensSelecionados, this.itenSelecionados)) {
            this.$emit('update:itensSelecionados', itensSelecionados);
          }
        }
      },
      deep: true
    }
  },

  mounted: function() {
    this.uuid = this.gerarUuid();
  },

  template: '#CampoBeneficiamentoSelecaoMultiplaInclusiva-template'
});
