Vue.component('trocas-devolucoes-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de contas recebidas.
     * @type {Object}
     */
    filtro: {
      required: false,
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
    },

    /**
     * Indica se o relatório será agrupado pelo funcionário.
     * @type {?boolean}
     */
    agruparPorFuncionario: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    },

    /**
     * Indica se o relatório será agrupado pelo funcionário associado ao cliente.
     * @type {?boolean}
     */
    agruparPorFuncionarioAssociado: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarBooleanOuVazio
    }
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          id: null,
          idPedido: null,
          idCliente: null,
          nomeCliente: null,
          idsFuncionario: [],
          idsFuncionarioAssociadoCliente: [],
          idProduto: null,
          tipo: 0,
          situacao: 0,
          periodoTrocaInicio: null,
          periodoTrocaFim: null,
          idGrupoProduto: 0,
          idSubgrupoProduto: 0,
          tipoPedido: null,
          idOrigem: 0,
          idSetor: 0,
          idTipoPerda: null,
          alturaMinima: null,
          alturaMaxima: null,
          larguraMinima: null,
          larguraMaxima: null
        },
        this.filtro
      ),
      grupoAtual: null,
      subgrupoAtual: null,
      origemAtual: null,
      setorAtual: null,
      tipoAtual: null,
      situacaoAtual: null,
      tipoPerdaAtual: null
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
     * Retorna os itens para o controle de vendedores.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedores: function () {
      return Servicos.Funcionarios.obterVendedores(null, null);
    },

    /**
     * Retorna os itens para o controle de vendedores associados à clientes.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroVendedoresAssociadosAClientes: function () {
      return Servicos.Funcionarios.obterAtivosAssociadosAClientes();
    },

    /**
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGrupos: function () {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @param {?Object} filtro O filtro para a busca de subgrupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgrupos: function (filtro) {
      return Servicos.Produtos.Subgrupos.obterParaControle((filtro || {}).idGrupoProduto);
    },

    /**
     * Retorna os itens para o controle de tipos de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposPedido: function () {
      return Servicos.Pedidos.obterTiposPedido();
    },

    /**
     * Retorna os itens para o controle de tipos de pedido.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroOrigemTrocaDevolucao: function () {
      return Servicos.Estoques.TrocasDevolucoes.Origens.obterParaFiltro();
    },

    /**
     * Recupera a lista de setores para o controle de filtro.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSetores: function () {
      return Servicos.Producao.Setores.obterParaControle();
    },

    /**
     * Recupera os tipos de trocas/devoluções.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterTipos: function () {
      return Servicos.Estoques.TrocasDevolucoes.obterTipos();
    },

    /**
     * Recupera as situações de trocas/devoluções.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterSituacoes: function () {
      return Servicos.Estoques.TrocasDevolucoes.obterSituacoes();
    }
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgrupos}
     *
     * @typedef filtroSubgrupos
     * @property {?number} idGrupoProduto O ID do grupo de produto.
     */
    filtroSubgrupos: function () {
      return {
        idGrupoProduto: (this.filtroAtual || {}).idGrupoProduto || 0
      };
    },

    /**
     * Propriedade computada que normaliza o valor da propriedade 'agruparPorFuncionario'
     * e que atualiza a propriedade principal em caso de alteração do valor.
     * @type {boolean}
     */
    agruparPorFuncionarioAtual: {
      get: function () {
        return this.agruparPorFuncionario || false;
      },
      set: function (valor) {
        if (valor !== this.agruparPorFuncionario) {
          this.$emit('update:agruparPorFuncionario', valor);
        }
      }
    },

    /**
     * Propriedade computada que normaliza o valor da propriedade 'agruparPorFuncionarioAssociado'
     * e que atualiza a propriedade principal em caso de alteração do valor.
     * @type {boolean}
     */
    agruparPorFuncionarioAssociadoAtual: {
      get: function () {
        return this.agruparPorFuncionarioAssociado || false;
      },
      set: function (valor) {
        if (valor !== this.agruparPorFuncionarioAssociado) {
          this.$emit('update:agruparPorFuncionarioAssociado', valor);
        }
      }
    }
  },

  mounted: function () {
    this.filtroAtual.situacao = 0;
    this.filtroAtual.tipo = 0;
  },

  watch: {
    /**
     * Observador para a variável 'grupoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoProduto = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'subgrupoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubgrupoProduto = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'origemAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    origemAtual: {
      handler: function (atual) {
        this.filtroAtual.idOrigem = atual ? atual.id : null;
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
     * Observador para a variável 'tipoPerdaAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoPerdaAtual: {
      handler: function (atual) {
        this.filtroAtual.idTipoPerda = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstTrocaDevolucao-Filtro-template'
});
