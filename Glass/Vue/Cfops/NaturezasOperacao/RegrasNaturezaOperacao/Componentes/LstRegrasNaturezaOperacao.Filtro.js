Vue.component('regras-natureza-operacao-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de regras de natureza de operação.
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
          idLoja: null,
          idNaturezaOperacao: null,
          idTipoCliente: null,
          idGrupoProduto: null,
          idSubgrupoProduto: null,
          idCorVidro: null,
          idCorFerragem: null,
          idCorAluminio: null,
          espessura: null,
          ufsDestino: null
        },
        this.filtro
      ),
      lojaAtual: {},
      naturezaOperacaoAtual: {},
      tipoClienteAtual: {},
      grupoProdutoAtual: {},
      subgrupoProdutoAtual: {},
      corVidroAtual: {},
      corFerragemAtual: {},
      corAluminioAtual: {}
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
     * Retorna os itens para o controle de tipos de cliente.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTipoCliente: function () {
      return Servicos.Clientes.Tipos.obterParaControle(true);
    },

    /**
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGrupoProduto: function () {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @param {?Object} filtro O filtro para a busca de subgrupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgrupoProduto: function (filtro) {
      return Servicos.Produtos.Subgrupos.obterParaControle((filtro || {}).idGrupoProduto);
    },

    /**
     * Retorna os itens para o controle de cor de vidro.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCorVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cor de ferragem.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCorFerragem: function () {
      return Servicos.Produtos.CoresFerragem.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cor de alumínio.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCorAluminio: function () {
      return Servicos.Produtos.CoresAluminio.obterParaControle();
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
    }
  },

  watch: {
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
     * Observador para a variável 'naturezaOperacaoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    naturezaOperacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.idNaturezaOperacao = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'tipoClienteAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    tipoClienteAtual: {
      handler: function (atual) {
        this.filtroAtual.idTipoCliente = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'grupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoProduto = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'subgrupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubgrupoProduto = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corVidroAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corVidroAtual: {
      handler: function (atual) {
        this.filtroAtual.idCorVidro = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corFerragemAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corFerragemAtual: {
      handler: function (atual) {
        this.filtroAtual.idCorFerragem = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corAluminioAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corAluminioAtual: {
      handler: function (atual) {
        this.filtroAtual.idCorAluminio = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstRegrasNaturezaOperacao-Filtro-template'
});
