Vue.component('campo-beneficiamento-quantidade', {
  mixins: [Mixins.Objetos, Mixins.CampoBeneficiamento],
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
    },

    /**
     * Indica se o controle deve criar o beneficiamento no momento da seleção,
     * caso o item selecionado ainda esteja null.
     * @type {?boolean}
     */
    criarBeneficiamentoSeNull: {
      required: false,
      twoWay: false,
      default: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function () {
    return {
      quantidade: ((this.itensSelecionados || [])[0] || {}).quantidade || 0,
    };
  },

  watch: {
    /**
     * Observador para a variável 'itensSelecionados'.
     * Atualiza o item selecionado com a quantidade do controle, se possível.
     */
    itensSelecionados: {
      handler: function (atual) {
        if (atual && atual.length && atual[0].quantidade !== this.quantidade) {
          atual[0].quantidade = this.quantidade
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'quantidade'.
     * Atualiza os itens selecionados com a quantidade selecionada.
     */
    quantidade: function (atual) {
      var itensSelecionados = null;

      if (this.quantidade > 0 || !this.criarBeneficiamentoSeNull) {
        if (this.itensSelecionados
          && this.itensSelecionados.length
          && Object.keys(this.itensSelecionados[0]).length) {

          itensSelecionados = this.itensSelecionados;
        } else if (this.criarBeneficiamento) {
          itensSelecionados = [
            this.criarBeneficiamento({
              id: this.beneficiamento.id,
              quantidade: 0
            })
          ];
        }

        if (itensSelecionados && itensSelecionados.length) {
          itensSelecionados[0].quantidade = atual;
        }
      }

      if (!this.equivalentes(itensSelecionados, this.itensSelecionados)) {
        this.$emit('update:itensSelecionados', itensSelecionados);
      }
    }
  },

  template: '#CampoBeneficiamentoQuantidade-template'
});
