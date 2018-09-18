Vue.component('caixa-diario-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de caixa diário.
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
          idLoja: null,
          idFuncionario: null,
          data: null
        },
        this.filtro
      ),
      lojaAtual: null,
      funcionarioAtual: null
    };
  },

  methods: {
    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual, true);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }

      if (this.lojaAtual.id !== this.filtro.idLoja) {
        this.$emit('loja-alterada', this.lojaAtual.id);
      }
    },

    /**
     * Retorna os itens para o controle de funcionários.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroFuncionarios: function() {
      return Servicos.Funcionarios.obterCaixaDiario();
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se o filtro por loja pode ser alterado.
     * @type {boolean}
     */
    permitirAlterarLoja: function () {
      return this.configuracoes
        && this.configuracoes.controleFinanceiroRecebimento
        && this.configuracoes.controleFinanceiroPagamento
        && this.configuracoes.controleCaixaDiario;
    },

    /**
     * Propriedade computada que indica se o filtro por data pode ser alterado.
     * @type {boolean}
     */
    permitirAlterarData: function () {
      return this.configuracoes
        && this.configuracoes.alterarDataConsulta;
    },

    /**
     * Propriedade computada que indica se o filtro por funcionário pode ser alterado.
     * @type {boolean}
     */
    permitirAlterarFuncionario: function () {
      return this.configuracoes
        && this.configuracoes.controleFinanceiroRecebimento;
    }
  },

  watch: {
    /**
     * Observador para a propriedade 'configuracoes'.
     * Atualiza o filtro com a loja atual do usuário, a data atual e o usuário logado.
     */
    configuracoes: {
      handler: function (atual) {
        this.lojaAtual = atual ? this.clonar(atual.lojaUsuario) : null;
        this.funcionarioAtual = atual ? this.clonar(atual.usuario) : null;
        this.filtroAtual.data = new Date();

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
    }
  },

  template: '#LstCaixaDiario-Filtro-template'
});
