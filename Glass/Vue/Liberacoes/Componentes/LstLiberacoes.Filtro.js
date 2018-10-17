Vue.component('liberacao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de liberações.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function() {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          idPedido: null,
          numeroNfe: null,
          idFuncionario: null,
          idCliente: null,
          nomeCliente: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          situacao: null,
          idLoja: null,
          periodoCancelamentoInicio: null,
          periodoCancelamentoFim: null,
          liberacaoComSemNotaFiscal: null
        },
        this.filtro
      ),
      liberadorAtual: null,
      lojaAtual: null,
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function() {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    },

    /**
     * Retorna os itens para o controle de liberadores de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroLiberadores: function() {
      return Servicos.Funcionarios.obterLiberadores();
    }
  },

  watch: {
    /**
     * Observador para a variável 'liberadorAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    liberadorAtual: {
      handler: function(atual) {
        this.filtroAtual.idFuncionario = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function (atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstLiberacoes-Filtro-template'
});
