Vue.component('campo-beneficiamento-selecao-multipla-exclusiva', {
  inheritAttrs: false,
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
    }
  },

  data: function () {
    return {
      itensSelecionadosAtuais: this.itensSelecionados,
      selecionando: false
    };
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
     * Observador para a variável 'itensSelecionadosAtuais'.
     * Garante que apenas um item será selecionado no controle interno.
     */
    itensSelecionadosAtuais: function (atual, anterior) {
      if (this.selecionando) {
        return;
      }

      try {
        var itens;
        this.selecionando = true;

        if (!atual || !atual.length) {
          itens = null;
        } else if (atual.length === 1) {
          itens = atual;
        } else {
          var antigos = anterior.map(function (item) {
            return item.id;
          });

          atual = atual.filter(function (item) {
            return antigos.indexOf(item.id) === -1;
          });

          if (!this.equivalentes(atual, this.itensSelecionadosAtuais)) {
            this.itensSelecionadosAtuais = atual;
          }

          itens = atual;
        }

        if (!this.equivalentes(itens, this.itensSelecionados)) {
          this.$emit('update:itensSelecionados', itens);
        }
      } finally {
        this.selecionando = false;
      }
    }
  },

  template: '#CampoBeneficiamentoSelecaoMultiplaExclusiva-template'
});
