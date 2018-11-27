Vue.component('funcionario-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de funcionarios.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de contas recebidas.
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
          nome: null,
          apenasRegistrados: null,
          periodoDataNascimentoInicio: null,
          periodoDataNascimentoFim: null,
          refresh_: 0
        },
        this.filtro
      ),
      lojaAtual: null,
      situacaoAtual: null,
      tipoFuncionarioAtual: null,
      setorAtual:null,
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
    },

    /**
     * Retorna os itens para o controle de situações do funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacao: function () {
      return Servicos.Funcionarios.obterSituacoes();
    },

    /**
     * Retorna os itens para o controle de tipo de funcionário.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposFuncionario: function () {
      return Servicos.Funcionarios.obterTiposFuncionario();
    },

    /**
     * Retorna os itens para o controle de setores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSetor: function () {
      return Servicos.Producao.Setores.obterParaControle(
       false,
       false
     );
    },
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
     * Observador para a variável 'setorAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    setorAtual: {
      handler: function (atual) {
        this.filtroAtual.idSetor = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoFuncionarioAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoFuncionarioAtual: {
      handler: function (atual) {
        this.filtroAtual.idTipoFuncionario = atual ? atual.id : null;
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
  },

  template: '#LstFuncionarios-Filtro-template'
});
