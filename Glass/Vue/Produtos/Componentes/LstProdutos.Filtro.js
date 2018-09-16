Vue.component('produtos-filtros', {
  mixins: [Mixins.Clonar, Mixins.Merge, Mixins.Comparar],
  props: {
    /**
     * Filtros selecionados para a lista de produtos.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Objeto com as configurações da tela de produtos.
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
          codigo: null,
          descricao: null,
          situacao: null,
          idGrupo: null,
          idSubgrupo: null,
          valorAlturaInicio: null,
          valorAlturaFim: null,
          valorLarguraInicio: null,
          valorLarguraFim: null,
          ordenacaoFiltro: null
        },
        this.filtro
      ),
      grupoAtual: null,
      subgrupoAtual: null
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
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGrupos: function() {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @param {?number} idGrupoProduto O ID do grupo de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgrupos: function (idGrupoProduto) {
      return Servicos.Produtos.Subgrupos.obterParaControle(idGrupoProduto);
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
        idGrupoProduto: (this.filtroAtual || {}).idGrupo || 0
      };
    }
  },

  mounted: function () {
    this.filtroAtual.situacao = 1;
  },

  watch: {
    /**
     * Observador para a variável 'grupoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    grupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupo = atual ? atual.id : null;
      },
      deep: true
    },

    /**
     * Observador para a variável 'subgrupoAtual'.
     * Atualiza o filtro com o ID do item selecionado.
     */
    subgrupoAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubgrupo = atual ? atual.id : null;
      },
      deep: true
    }
  },

  template: '#LstProdutos-Filtro-template'
});
