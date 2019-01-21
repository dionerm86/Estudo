Vue.component('movimentacoes-liberacoes', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de movimentações de liberações.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {          
          idCliente: null,
          nomeCliente: null,
          idFuncionario: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          situacao: null
        },
        this.filtro
      ),
      situacaoAtual: null,
      funcionarioAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de movimentações de liberações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFuncionarios: function () {
      return Servicos.Liberacoes.obterFuncionarios();
    },

    /**
     * Retorna os itens para o controle de movimentações de liberações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function () {
      return Servicos.Liberacoes.obterSituacoes();
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'funcionarioAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    funcionarioAtual: {
      handler: function (atual) {
        this.filtroAtual.idFuncionario = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstMovimentacoesLiberacoes-Filtro-template'
});
