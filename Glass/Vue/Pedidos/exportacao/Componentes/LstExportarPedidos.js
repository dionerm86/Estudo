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
     * @returns {Promise} uma Promise com o resultado da exportação.
     */
    exportar: function () {
      this.iniciarExportacao();
      return Servicos.Pedidos.Exportacao.exportar(this.dadosExportacao);
    },

    /**
     * Prepara os dados para exportação de pedidos.
     */
    iniciarExportacao: function () {
      this.dadosExportacao = {
        idFornecedor: this.fornecedorAtual.id,
        pedidos: this.gerarDadosPedidos()
      };
    },

    /**
     * Preenche o objeto contendo os dados dos pedidos para envio ao controller de exportação.
     * @return {Object} Um objeto com os dados dos pedidos para exportação.
     */
    gerarDadosPedidos: function () {
      var vm = this;
      var pedidos = [];
      this.pedidosExportar.forEach(function (idPedidoExportar) {
        var produtosPedidoExportar = vm.configuracoes.exibirProdutos 
            ? vm.$refs['produtosPedido' + idPedidoExportar].produtosPedidoExportar 
            : vm.$refs['produtosPedido' + idPedidoExportar].$refs.lista.itens;

        pedidos.push({
          idPedido: idPedidoExportar,
          exportarBeneficiamento: vm.beneficiamentosExportar.indexOf(idPedidoExportar) > -1,
          idsProdutoPedido: produtosPedidoExportar
        });
      });

      return pedidos;
    },

    /**
     * Consulta a situação da exportação de um pedido.
     * @param {number} idPedido O identificador do pedido.
     * @returns {Promise} Uma promise com o resultado da operação.
     */
    consultarSituacao: function (idPedido) {
      return Servicos.Pedidos.Exportacao.consultarSituacao(idPedido, this.fornecedorAtual.id);
    },

    /**
     * Obtem os itens para o controle de fornecedores.
     * @returns {Promise} Uma promise com o resultado da busca.
     */
    obterItensControleFornecedores: function () {
      return Servicos.Fornecedores.obterParaControleExportacao();
    },

    /**
     * Verifica se os produtos de um determinado pedido estão sendo exibidos.
     * @param {number} indice O índice atual em que será realizada a verificação.
     * @returns {boolean} Um valor que indica se os produtos estão sendo exibidos no indice informado.
     */
    verificarExibicaoProdutos: function (indice) {
      return this.produtosEmExibicao.indexOf(indice) > -1;
    },

    /**
     * Retorna o número de colunas da lista paginada.
     * @returns {number} O número de colunas presentes na lista paginada.
     */
    obterNumeroColunasLista: function () {
      return this.$refs.lista.numeroColunas();
    },

    /**
     * Alterna a exibição dos produtos.
     * @param {number} indice O índice que terá a exibição dos produtos alternada.
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
     * Verifica se um dado pedido está marcado para exportação.
     * @param {number} id O identificador do pedido.
     */
    verificarMarcado: function (id) {
      var vm = this;
      var itens = this.$refs['produtosPedido' + id].$refs.lista.itens;
      if (this.pedidosExportar.indexOf(id) > -1) {
        itens.forEach(function (item) {
          vm.$refs['produtosPedido' + id].produtosPedidoExportar.push(item.id);
        });
      } else {
        vm.$refs['produtosPedido' + id].produtosPedidoExportar = [];
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
