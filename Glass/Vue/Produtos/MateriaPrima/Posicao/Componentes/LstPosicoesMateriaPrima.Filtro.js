Vue.component('posicoes-materia-prima-filtros', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de posições de matéria prima.
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
          idsRota: null,
          tiposPedido: null,
          situacoesPedido: null,
          nomeCliente: null,
          periodoEntregaPedidoInicio: null,
          periodoEntregaPedidoFim: null,
          idsCorVidro: null,
          espessura: null,
          buscarApenasEstoqueDisponivelNegativo: null
        },
        this.filtro
      )
    };
  },

  methods: {
    /**
     * Retorna os itens para o controle de situações.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterItensFiltroSituacoesPedidoPcp: function () {
      return Servicos.Pedidos.obterSituacoesPedidoPcp();
    },

    /**
     * Retorna os itens para o controle de tipos.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterTiposFiltroSituacoesPedidoPcp: function (filtro) {
      return Servicos.Pedidos.obterTiposPedidoPcp();
    },

    /**
     * Retorna os itens para o controle de rotas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterRotas: function () {
      return Servicos.Rotas.obterFiltro();
    },

    /**
     * Retorna os itens para o controle de cores de vidro.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    obterCoresVidro: function () {
      return Servicos.Produtos.CoresVidro.obterParaControle();
    },

    /**
     * Atualiza o filtro com os dados selecionados na tela.
     */
    filtrar: function () {
      var novoFiltro = this.clonar(this.filtroAtual);
      if (!this.equivalentes(this.filtro, novoFiltro)) {
        this.$emit('update:filtro', novoFiltro);
      }
    }
  },

  template: '#LstPosicoesMateriaPrima-Filtro-template'
});
