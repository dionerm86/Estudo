Vue.component('movimentacoesestoquereal-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de estoques de produto.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },
  },

  data: function () {
    return {
      filtroAtual: this.merge(
        {
          idLoja: null,
          codigoProduto: null,
          descricaoProduto: null,
          periodoMovimentacaoInicio: null,
          periodoMovimentacaoFim: null,
          tipoMovimentacao: null,
          situacaoProduto: null,
          idsGrupoProduto: [],
          idsSubgrupoProduto: [],
          codigoOtimizacao: null,
          idCorVidro: null,
          idCorFerragem: null,
          idCorAluminio: null,
          apenasLancamentosManuais: false,
          naoBuscarProdutosComEstoqueZerado: false,
          usarValorFiscalDoProdutoNoInventario: false,
          ordenacaoFiltro: null
        },
        this.filtro
      ),
      lojaAtual: null,
      tipoCor: null,
      corVidroAtual: null,
      corFerragemAtual: null,
      corAluminioAtual: null,
    };
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgruposProduto}
     *
     * @typedef filtroSubgruposProduto
     * @property {?number} idGrupoProduto O ID do grupo de produto.
     */
    filtroSubgruposProduto: function () {
      return {
        idGrupoProduto: (this.filtroAtual || {}).idsGrupoProduto || 0
      };
    }
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
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGruposProduto: function () {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgruposProduto: function () {
      var idsGrupoProduto = (this.filtroAtual || {}).idsGrupoProduto;

      if (idsGrupoProduto.length == 0) {
        return;
      }

      for (var i = 0; i < idsGrupoProduto.length; i++) {
        return Servicos.Produtos.Subgrupos.obterParaControle(idsGrupoProduto[i]);
      }
    },

    /**
     * Retorna os itens para o controle de cores de vidro.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCoresVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cores de ferragem.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCoresFerragem: function () {
      return Servicos.Produtos.CoresFerragem.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de cores de alumínio.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroCoresAluminio: function () {
      return Servicos.Produtos.CoresAluminio.obterParaControle();
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
     * Observador para a variável 'grupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idsGrupoProduto = atual ? atual.id : null;
        this.filtroAtual.idsSubgrupoProduto = null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'subgrupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idsSubgrupoProduto = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corVidro'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    corVidroAtual: {
      handler: function (atual) {
        this.filtroAtual.idCorVidro = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'corFerragem'.
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

  template: '#ListaMovimentacoesEstoqueReal-Filtro-template'
});
