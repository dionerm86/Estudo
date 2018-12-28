Vue.component('compras-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de compras.
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
          idPedido: null,
          idCotacaoCompra: null,
          notaFiscal: null,
          idFornecedor: null,
          nomeFornecedor: null,
          observacao: null,
          situacao: null,
          atrasada: null,
          periodoCadastroInicio: null,
          periodoCadastroFim: null,
          periodoEntregaFabricaInicio: null,
          periodoEntregaFabricaFim: null,
          periodoSaidaInicio: null,
          periodoSaidaFim: null,
          periodoFinalizacaoInicio: null,
          periodoFinalizacaoFim: null,
          periodoEntradaInicio: null,
          periodoEntradaFim: null,
          idsGrupoProduto: [],
          idSubgrupoProduto: null,
          codigoProduto: null,
          descricaoProduto: null,
          idLoja: null,
          centroDeCustoDivergente: null
        },
        this.filtro      
      ),
      lojaAtual: null,
      situacaoAtual: null,
      subgrupoProdutoAtual: null
    };
  },

  computed: {
    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgrupos}
     *
     * @typedef filtroSubgrupos
     * @property {?number} idsGrupoProduto O ID do grupo de produto.
     */
    filtroSubgrupos: function () {
      return {
        idsGrupoProduto: (this.filtroAtual || {}).idsGrupoProduto || []
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
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesCompra: function () {
      return Servicos.Compras.obterSituacoesParaControle(null, null);
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
     * @param {Object} filtro O filtro base para recuperar os ids de grupos de produto selecionados.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgruposProduto: function (filtro) {
      var idsGrupoProduto = (filtro || {}).idsGrupoProduto || [];
      return Servicos.Produtos.Subgrupos.obterVariosParaControle(idsGrupoProduto);
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
     * Observador para a variável 'subgrupoProdutoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubgrupoProduto = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstCompras-Filtro-template'
});
