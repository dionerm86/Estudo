Vue.component('campo-beneficiamento-lista-selecao', {
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
      selecionando: false,
      itemSelecionadoAtual: (this.itensSelecionados || [])[0] || null
    };
  },

  methods: {
    /**
     * Retorna os itens para serem usados na lista de seleção.
     * @param {Array} filtro Os filhos do beneficiamento atual, se houver.
     * @returns {Promise} Uma promise com os filhos do beneficiamento atual.
     */
    buscarItens: function (filtro) {
      return Promise.resolve({
        data: filtro
      });
    }
  },

  computed: {
    /**
     * Propriedade que retorna os filhos do beneficiamento atual, se existirem,
     * para serem utilizados como itens a serem selecionados.
     * @type {!Array}
     */
    filtro: function () {
      return (this.beneficiamento || {}).filhos || [];
    }
  },

  watch: {
    /**
     * Observador para a variável 'beneficiamento'.
     * Reinicia os valores selecionados no controle se o beneficiamento for alterado.
     */
    beneficiamento: {
      handler: function () {
        this.itemSelecionadoAtual = null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'itensSelecionados'.
     * Atualiza os itens selecionados atualmente, em caso de alteração externa.
     */
    itensSelecionados: {
      handler: function (atual) {
        if (this.selecionando) {
          return;
        }

        try {
          this.selecionando = true;

          var novo = atual && atual.length
            ? this.filtro
              .filter(function (item) {
                return item.id === atual[0].id;
              })
              .map(function (item) {
                return {
                  id: item.id,
                  nome: item.nome
                };
              })
            : [];

          novo = novo[0] || null;

          if (!this.equivalentes(novo, this.itemSelecionadoAtual)) {
            this.itemSelecionadoAtual = novo;
          }
        } finally {
          this.selecionando = false;
        }
      },
      deep: true
    },

    /**
     * Observador para a variável 'itemSelecionadoAtual'.
     * Altera a propriedade com os itens selecionados no controle.
     */
    itemSelecionadoAtual: {
      handler: function (atual) {
        if (this.selecionando) {
          return;
        }

        try {
          this.selecionando = true;
          var item = null;
          if (atual) {
            item = [
              this.criarBeneficiamento({
                id: atual.id
              })
            ];
          }

          if (!this.equivalentes(item, this.itensSelecionados)) {
            this.$emit('update:itensSelecionados', item);
          }
        } finally {
          this.selecionando = false;
        }
      },
      deep: true
    }
  },

  template: '#CampoBeneficiamentoListaSelecao-template'
});
