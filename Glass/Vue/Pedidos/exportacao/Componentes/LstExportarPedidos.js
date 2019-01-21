const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('IdPedido', 'ASC')],

  data: {
    configuracoes: {},
    filtro: {
      periodoCadastroInicio: new Date(),
      periodoCadastroFim: new Date()
    },
    fornecedorAtual: null,
    dadosExportacao: {},
    pedidosExportar: [],
    beneficiamentosExportar: [],
    produtosEmExibicao: []
  },

  methods: {
    /**
     * Busca os pedidos para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca de pedidos.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Pedidos.Exportacao.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Exporta um ou mais pedidos para um dado fornecedor.
     */
    exportar: function () {
      return Servicos.Pedidos.Exportacao.exportar(this.dadosExportacao);
    },

    /**
     * Consulta a situação da exportação de um pedido.
     * @param {number} idPedido O identificador do pedido.
     */
    consultarSituacao: function (idPedido) {
      return Servicos.Pedidos.Exportacao.consultarSituacao(idPedido, this.fornecedorAtual.id);
    },

    /**
     * Obtem os itens para o controle de fornecedores.
     */
    obterItensControleFornecedores: function () {
      return Servicos.Fornecedores.obterParaControleExportacao();
    },

    /**
     * Verifica se os produtos de um determinado pedido estão sendo exibidos.
     */
    exibindoProdutos: function (indice) {
      return this.produtosEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @type {number}
     */
    numeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Alterna a exibição dos produtos.
     */
    alternarExibicaoProdutos: function (indice) {
      var i = this.produtosEmExibicao.indexOf(indice);

      if (i > -1) {
        this.produtosEmExibicao.splice(i, 1);
      } else {
        this.produtosEmExibicao.push(indice);
      }
    },
    
    /**
     * Marca todos os pedidos da lista paginada para exportação.
     */
    marcarTodosOsPedidosParaExportacao: function (event) {
      if (event.target.checked) {
        var itens = this.$refs.lista.itens;
        for (var i = 0; i < itens.length; i++) {
          this.pedidosExportar.push(itens[i].id);
        }
      }
      else {
        for (var i = 0; i < this.pedidosExportar.length; i++) {
          this.pedidosExportar.splice(i, 1);
        }
      }
    },

    /**
     * Força a atualização da lista de pedidos, com base no filtro atual.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    }
  },

  mounted: function () {
    var vm = this;
    Servicos.Pedidos.Exportacao.obterConfiguracoes()
      .then(function (resposta) {
        vm.configuracoes = resposta.data;
      });
  }
});
