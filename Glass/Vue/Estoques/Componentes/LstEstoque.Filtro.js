Vue.component('estoque-filtros', {
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

    /**
     * Objeto com as configurações da tela de estoques de produto.
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
          idLoja: null,
          codigoInternoProduto: null,
          descricaoProduto: null,
          idGrupoProduto: null,
          idsSubgrupoProduto: null,
          apenasComEstoque: null,
          apenasPosseTerceiros: null,
          apenasProdutosProjeto: null,
          idCorVidro: null,
          idCorFerragem: null,
          idCorAluminio: null,
          situacao: null,
          estoqueFiscal: null,
          aguardandoSaidaEstoque: null,
          ordenacaoFiltro: null
        },
        this.filtro
      ),
      lojaAtual: null,
      grupoProdutoAtual: null,
      corVidroAtual: null,
      corFerragemAtual: null,
      corAluminioAtual: null
    };
  },

  computed: {
    /**
     * Propriedade computada que indica se deverão ser exibidos dados fiscais
     */
    exibirEstoqueFiscal: function () {
      return GetQueryString('fiscal') == '1';
    },

    /**
     * Propriedade computada que indica se o filtro "Aguardando saída estoque" será exibido
     */
    exibirFiltroAguardandoSaidaEstoque: function () {
      return !this.exibirEstoqueFiscal
        && (!this.configuracoes.usarLiberacaoPedido || !this.configuracoes.marcarSaidaEstoqueAoLiberarPedido)
        && (this.configuracoes.usarLiberacaoPedido || !this.configuracoes.marcarSaidaEstoqueAutomaticaAoConfirmar);
    },

    /**
     * Propriedade computada que retorna o filtro de subgrupos de produto.
     * @type {filtroSubgruposProduto}
     *
     * @typedef filtroSubgruposProduto
     * @property {?number} idGrupoProduto O ID do grupo de produto.
     */
    filtroSubgruposProduto: function() {
      return {
        idGrupoProduto: (this.filtroAtual || {}).idGrupoProduto || 0
      };
    }
  },

  mounted: function () {
    this.filtroAtual.situacao = 1;
    this.filtroAtual.ordenacaoFiltro = 1;
    this.filtroAtual.estoqueFiscal = this.exibirEstoqueFiscal;
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
     * Retorna os itens para o controle de grupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroGruposProduto: function() {
      return Servicos.Produtos.Grupos.obterParaControle();
    },

    /**
     * Retorna os itens para o controle de subgrupos de produto.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSubgruposProduto: function (filtro) {
      return Servicos.Produtos.Subgrupos.obterParaControle((filtro || {}).idGrupoProduto);
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
     * Observador para a propriedade 'configuracoes'.
     * Atualiza o filtro com a loja atual do usuário.
     */
    configuracoes: {
      handler: function (atual) {
        this.lojaAtual = atual ? this.clonar(atual.lojaUsuario) : null;

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
      handler: function(atual) {
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
        this.filtroAtual.idGrupoProduto = atual ? atual.id : null;
        this.filtroAtual.idsSubgrupoProduto = null;
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

  template: '#LstEstoque-Filtro-template'
});
