Vue.component('exportacao-pedidos-produtos', {
  mixins: [Mixins.Objetos],
  props: {
    /**
     * Filtros selecionados para a lista de produtos de pedidos para exportação.
     * @type {Object}
     */
    filtro: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    },

    /**
     * Lista de pedidos marcados para exportação na lista de pedidos para exportação.
     * @type {Object}
     */
    pedidosMarcadosParaExportacao: {
      required: true,
      twoWay: true,
      validator: Mixins.Validacao.validarObjeto
    }
  },

  data: function () {
    return {
      produtosPedidoExportar: []
    };
  },

  methods: {
    /**
     * Função que busca busca os produtos do pedido para exportação.
     * @returns {Promise} Uma Promise com a busca dos itens, de acordo com o filtro.
     */
    buscarProdutos: function () {
      return Servicos.Pedidos.Produtos.obterListaProdutosPedidoExportacao(this.filtro.idPedido);
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

  template: '#LstExportarPedidos-Produtos-template'
});
