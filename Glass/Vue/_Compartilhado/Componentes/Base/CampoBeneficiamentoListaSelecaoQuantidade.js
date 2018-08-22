Vue.component('campo-beneficiamento-lista-selecao-quantidade', {
  mixins: [Mixins.Comparar],
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

  data: function() {
    return {
      itensSelecionadosAtuais: this.itensSelecionados || null
    };
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamento'.
     * Reinicia os valores selecionados no controle se o beneficiamento for alterado.
     */
    beneficiamento: {
      handler: function () {
        this.itensSelecionadosAtuais = null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionadosAtuais'.
     * Atualiza a propriedade com o item selecionado atualmente, se possível.
     */
    itensSelecionadosAtuais: {
      handler: function (atual) {
        var item = null;
        if (atual && atual.length && atual[0].quantidade > 0) {
          item = atual
        }

        if (!this.equivalentes(item, this.itensSelecionados)) {
          this.$emit('update:itensSelecionados', item);
        }
      },
      deep: true
    }
  },

  template: '#CampoBeneficiamentoListaSelecaoQuantidade-template'
});
