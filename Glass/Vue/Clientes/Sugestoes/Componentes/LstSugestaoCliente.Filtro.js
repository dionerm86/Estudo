Vue.component('sugestao-cliente-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de pedidos.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de pedidos.
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
          descricao: null,
          idFuncionario: null,
          idCliente: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          tipo: null,
          idPedido: null,
          idOrcamento: null,
          situacao: null,
          idRota: null,
          idVendedorAssociado: null
        },
        this.filtro
      ),
      funcionario: null,
      tipo: null,
      situacao: null,
      rota: null,
      vendedorAssociado: null
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
     * Retorna os itens para filtro de funcionários.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterFuncionariosAssociadosASugestoesClientes: function () {
      return Servicos.Funcionarios.obterSugestoesCliente();
    },

    /**
     * Retorna os itens para o filtro de tipos de sugestão.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposSugestaoCliente: function () {
      return Servicos.SugestaoCliente.obterTiposSugestaoCliente();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroRotas: function (id, codigo) {
      return Servicos.Rotas.obterFiltro(id, codigo);
    },

    /**
     * Retorna os itens para o controle de funcionários de finalização.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    recuperarVendedores: function() {
      return Servicos.Funcionarios.obterVendedores(null, true);
    },

    /**
     * Retorna os itens para o filtro de situação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.SugestaoCliente.situacoes();
    }
  },

  mounted: function () {
    var idPedido = GetQueryString('idPedido');
    var idOrcamento = GetQueryString('idOrcamento');
    var idCliente = GetQueryString('idCliente');

    if (idPedido) {
      this.filtroAtual.idPedido = idPedido;
    }

    if (idOrcamento) {
      this.filtroAtual.idOrcamento = idOrcamento;
    }

    if (idCliente) {
      this.filtroAtual.idCliente = idCliente;
    }
  },

  computed: {
    /**
     * Propriedade computada que indica se o filtro de clientes deve ser exibido.
     * @type {boolean}
     */
    exibirFiltroCliente: function () {
      var idCliente = GetQueryString('idCliente');
      return !idCliente;
    },

    /**
     * Propriedade computada que indica se o filtro de pedidos deve ser exibido.
     * @type {boolean}
     */
    exibirFiltroPedido: function () {
      var idPedido = GetQueryString('idPedido');
      return !idPedido;
    },

    /**
     * Propriedade computada que indica se o filtro de orçamentos deve ser exibido.
     * @type {boolean}
     */
    exibirFiltroOrcamento: function () {
      var idOrcamento = GetQueryString('idOrcamento');
      return !idOrcamento;
    },
  },

  watch: {
    /**
     * Observador para a variável 'funcionario'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    funcionario: {
      handler: function (atual) {
        this.filtroAtual.idFuncionario = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipo'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipo: {
      handler: function (atual) {
        this.filtroAtual.tipo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'situacao'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    situacao: {
      handler: function (atual) {
        this.filtroAtual.situacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'rota'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    rota: {
      handler: function (atual) {
        this.filtroAtual.idRota = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'vendedorAssociado'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    vendedorAssociado: {
      handler: function (atual) {
        this.filtroAtual.idVendedorAssociado = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstSugestaoCliente-Filtro-template'
});
