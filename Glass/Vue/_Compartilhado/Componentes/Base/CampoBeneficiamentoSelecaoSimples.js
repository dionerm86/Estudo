Vue.component('campo-beneficiamento-selecao-simples', {
  mixins: [Mixins.Comparar, Mixins.CampoBeneficiamento],
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
      selecionado: !!(this.itensSelecionados || [])[0],
    };
  },

  watch: {
    /**
     * Observador para a variável 'itensSelecionados'.
     * Atualiza o item selecionado com a quantidade do controle, se possível.
     */
    itensSelecionados: {
      handler: function (atual) {
        this.selecionado = !!(atual && atual.length);
      },
      deep: true
    },

    /**
     * Observador para a variável 'selecionado'.
     * Atualiza os itens selecionados, se houver marcação do controle.
     */
    selecionado: function (atual) {
      var item = null;

      if (atual) {
        item = [
          this.criarBeneficiamento({
            id: this.beneficiamento.id
          })
        ];
      }

      if (!this.equivalentes(item, this.itensSelecionados)) {
        this.$emit('update:itensSelecionados', item);
      }
    }
  },

  template: '#CampoBeneficiamentoSelecaoSimples-template'
});
