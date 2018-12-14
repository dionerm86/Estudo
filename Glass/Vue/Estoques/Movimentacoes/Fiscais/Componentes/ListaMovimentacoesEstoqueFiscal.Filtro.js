﻿Vue.component('movimentacoes-estoque-fiscal-filtros', {
  mixins: [Mixins.Data, Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de movimentações do estoque fiscal.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

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
          codigoProduto: null,
          descricaoProduto: null,
          ncm: null,
          periodoMovimentacaoInicio: null,
          periodoMovimentacaoFim: null,
          tipoMovimentacao: null,
          situacaoProduto: null,
          idCfop: null,
          numeroNotaFiscal: null,
          idGrupoProduto: null,
          idSubgrupoProduto: null,
          idCorVidro: null,
          idCorFerragem: null,
          idCorAluminio: null,
          apenasLancamentosManuais: false,
          naoBuscarProdutosComEstoqueZerado: false,
          usarValorFiscalDoProdutoNoInventario: false
        },
        this.filtro
      ),
      lojaAtual: null,
      corVidroAtual: null,
      corFerragemAtual: null,
      corAluminioAtual: null,
      idGrupoProdutoAtual: null,
      idSubgrupoProdutoAtual: null,
      tipoMovimentacaoAtual: null
    };
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
        idGrupoProduto: (this.filtroAtual || {}).idGrupoProduto || []
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
    obterItensFiltroSubgruposProduto: function (filtro) {
      var idGrupoProduto = (filtro || {}).idGrupoProduto || [];
      return Servicos.Produtos.Subgrupos.obterParaControle(idGrupoProduto);
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
    },

    /**
     * Retorna os itens para o controle de tipos de movimentação.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroTiposMovimentacao: function () {
      return Servicos.Estoques.Movimentacoes.TiposMovimentacao.obterParaControle();
    },
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
    },

    tipoMovimentacaoAtual: {
      handler: function (atual) {
        this.filtroAtual.tipoMovimentacao = atual ? atual.id : null;
      },
      deep: true
    },

    idGrupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idGrupoProduto = atual ? atual.id : null;
      }
    },

    idSubgrupoProdutoAtual: {
      handler: function (atual) {
        this.filtroAtual.idSubgrupoProduto = atual ? atual.id : null;
      }
    },

    configuracoes: {
      handler: function (atual) {
        //Inicialização dos filtros padronizados e das propriedades do objeto filtro.
        var dataAtual = new Date();
        this.filtroAtual.periodoMovimentacaoInicio = this.adicionarDias(dataAtual, -15);
        this.filtroAtual.periodoMovimentacaoFim = dataAtual;
        this.filtrar();

        var vm = this;

        this.$nextTick(function () {
          vm.filtrar();
        });
      },
      deep: true
    }
  },

  template: '#ListaMovimentacoesEstoqueFiscal-Filtro-template'
});
