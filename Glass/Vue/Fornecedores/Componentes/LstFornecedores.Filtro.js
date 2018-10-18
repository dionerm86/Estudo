Vue.component('fornecedor-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de fornecedores.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de fornecedores.
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
          nome: null,
          situacao: null,
          cpfCnpj: null,
          comCredito: null,
          idPlanoConta: null,
          idParcela: null,
          endereco: null,
          vendedor: null
        },
        this.filtro
      ),
      situacaoAtual: null,
      parcelaAtual: null,
      planoContaAtual: null
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
     * Retorna os itens para o controle de situações de fornecedor.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoes: function () {
      return Servicos.Fornecedores.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de parcelas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroParcelas: function () {
      return Servicos.Parcelas.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de planos de conta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroPlanosConta: function () {
      return Servicos.PlanosConta.obterParaControle(2);
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
     * Observador para a variável 'parcelaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    parcelaAtual: {
      handler: function (atual) {
        this.filtroAtual.idParcela = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'planoContaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    planoContaAtual: {
      handler: function (atual) {
        this.filtroAtual.idPlanoConta = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstFornecedores-Filtro-template'
});
