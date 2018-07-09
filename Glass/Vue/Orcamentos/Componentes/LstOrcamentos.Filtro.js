Vue.component('orcamento-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de orçamentos.
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
          idCliente: null,
          nomeCliente: null,
          idVendedor: null,
          telefone: null,
          idCidade: null,
          bairro: null,
          endereco: null,
          complemento: null,
          situacao: null,
          idLoja: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null
        },
        this.filtro
      ),
      situacaoAtual: null,
      lojaAtual: null,
      cidadeAtual: null
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
     * Retorna os itens para o controle de situações de orçamento.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesOrcamento: function() {
      return Servicos.Orcamentos.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de funcionários de finalização.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    recuperarVendedores: function() {
      return Servicos.Orcamentos.obterVendedores();
    }
  },

  watch: {
    /**
     * Observador para a variável 'situacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacaoAtual: {
      handler: function(atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'lojaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    lojaAtual: {
      handler: function(atual) {
        this.filtroAtual.idLoja = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'cidadeAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    cidadeAtual: {
      handler: function(atual) {
        this.filtroAtual.idCidade = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstOrcamentos-Filtro-template'
});
