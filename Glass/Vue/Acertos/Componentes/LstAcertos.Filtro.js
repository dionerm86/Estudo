Vue.component('acertos-filtros', {
  mixins: [Mixins.Clonar, Mixins.Patch, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de acertos.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de acertos.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          idPedido: null,
          idLiberacao: null,
          idCliente: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          idFormaPagamento: null,
          numeroNfe: null,
          protesto: null
        },
        this.filtro
      ),
      formaPagamentoAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual, true);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de formas de pagamento.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFormasPagamento: function () {
      return Servicos.FormasPagamento.obterFiltroContasRecebidas();
    }
  },

  watch: {
    /**
     * Observador para a variável 'formaPagamentoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    formaPagamentoAtual: {
      handler: function(atual) {
        this.filtroAtual.idFormaPagamento = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a propriedade 'configuracoes'.
     * Inicia o filtro de protesto.
     */
    configuracoes: {
      handler: function (atual) {
        this.filtroAtual.protesto = 0;
        this.filtrar();
      },
      deep: true
    }
  },

  template: '#LstAcertos-Filtro-template'
});
