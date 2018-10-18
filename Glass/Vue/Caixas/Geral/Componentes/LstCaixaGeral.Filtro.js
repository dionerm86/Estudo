Vue.component('caixa-geral-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de caixa geral.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela.
     * @type {!Object}
     */
    configuracoes: {
      required: true,
      twoWay: false,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          idLoja: null,
          idFuncionario: null,
          tipo: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          valor: null,
          apenasDinheiro: null,
          apenasCheque: null,
          apenasEntradaExcetoEstorno: null
        },
        this.filtro
      ),
      lojaAtual: null,
      funcionarioAtual: null,
      tipoAtual: null
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
     * Retorna os itens para o controle de funcionários.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFuncionarios: function() {
      return Servicos.Funcionarios.obterFinanceiros();
    },

    /**
     * Retorna os itens para o controle de tipos de movimentação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposMovimentacao: function () {
      return Servicos.Caixas.Geral.obterTiposMovimentacao();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se será permitido filtrar por período ao invés de uma data única.
     * @type {boolean}
     */
    permitirFiltrarPeriodo: function () {
      return this.configuracoes
        && this.configuracoes.controleFinanceiroRecebimento
        && this.configuracoes.controleFinanceiroPagamento;
    },

    /**
     * Propriedade computada que indica se o filtro por funcionário pode ser alterado.
     * @type {boolean}
     */
    permitirAlterarFuncionario: function () {
      return this.configuracoes.permitirFiltrarFuncionario;
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza os filtros com valores padrões.
     */
    configuracoes: {
      handler: function (atual) {
        this.filtroAtual.periodoCadastroInicio = new Date();
        this.filtroAtual.periodoCadastroFim = new Date();

        if (!this.configuracoes.permitirFiltrarFuncionario) {
          this.funcionarioAtual = atual ? this.clonar(atual.usuario) : null;
        }

        var vm = this;

        this.$nextTick(function () {
          vm.filtrar();
        });
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
    },

    /**
     * Observador para a variável 'tipoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'filtroAtual.periodoCadastroInicio'.
     * Replica o filtro para a data final.
     */
    'filtroAtual.periodoCadastroInicio': {
      handler: function (atual) {
        if (this.configuracoes && !(this.configuracoes.controleFinanceiroRecebimento && this.configuracoes.controleFinanceiroPagamento)) {
          this.filtroAtual.periodoCadastroFim = atual;
        }
      },
      deep: true
    }
  },

  template: '#LstCaixaGeral-Filtro-template'
});
