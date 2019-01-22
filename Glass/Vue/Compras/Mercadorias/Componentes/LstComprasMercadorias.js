const app = new Vue({
  el: '#app',
  mixins: [Mixins.Objetos, Mixins.FiltroQueryString, Mixins.OrdenacaoLista('c.IdCompra', 'desc')],

  data: {
    filtro: {}
  },

  methods: {
    /**
     * Busca as compras de mercadorias para exibição na lista.
     * @param {!Object} filtro O filtro utilizado para a busca dos itens.
     * @param {!number} pagina O número da página que será exibida na lista.
     * @param {!number} numeroRegistros O número de registros que serão exibidos na lista.
     * @param {string} ordenacao A ordenação que será usada para a recuperação dos itens.
     * @returns {Promise} Uma promise com o resultado da busca dos itens.
     */
    obterLista: function (filtro, pagina, numeroRegistros, ordenacao) {
      var filtroUsar = this.clonar(filtro || {});
      return Servicos.Compras.ComprasMercadorias.obterLista(filtroUsar, pagina, numeroRegistros, ordenacao);
    },

    /**
     * Obtem o link para inserção de compras de mercadorias.
     */
    obterLinkInserirCompraMercadoria: function () {
      return '../Cadastros/CadCompraPcp.aspx';
    },

    /**
     * Exibe os dados detalhados da compra de mercadoria em um relatório.
     * @param {Boolean} exportarExcel O identificador da compra de mercadoria que será visualizada.
     */
    abrirRelatorioComprasMercadorias: function (id) {
      var url = '../Relatorios/RelBase.aspx?rel=Compra&idCompra=' + id;
      this.abrirJanela(600, 800, url);
    },

    /**
     * Obtem o link para cancelar compras de mercadorias.
     * @param {Object} item A compra de mercadoria que será cancelada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    cancelar: function (item) {
      this.abrirJanela(150, 420, "../Utils/SetMotivoCancCompra.aspx?idCompra=" + item.id);
    },

    /**
     * Abre um popup com o gerenciamento de fotos para a compra.
     * @param {Object} item A compra que terá as fotos gerenciadas.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    abrirGerenciamentoDeFotos: function (item) {
      this.abrirJanela(600, 700, '../Cadastros/CadFotos.aspx?id=' + item.id + '&tipo=compra');
    },

    /**
     * Obtem o link para edição de compras de mercadorias.
     * @param {number} id O identificador da compra de mercadoria que será editada.
     */
    obterLinkEditarCompraMercadoria: function (id) {
      return this.obterLinkInserirCompraMercadoria() + '?idCompra=' + id;
    },

    /**
     * Redireciona o usuário para uma tela onde poderá ser gerada uma nota fiscal para a compra.
     * @param {Object} item A compra que terá uma nota fiscal gerada.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    gerarNotaFiscal: function (item) {
      if (!confirm('Deseja gerar a nota fiscal para a compra de número ' + item.id + '?')) {
        return;
      }

      redirectUrl('../Cadastros/CadNotaFiscalGerarCompra.aspx?idCompra=' + item.id);
    },

    /**
     * Reabre uma compra já finalizada.
     * @param {Object} item A compra que será reaberta.
     * @returns {Promise} Uma Promise com o resultado da busca.
     */
    reabrir: function (item) {
      if (!confirm('Deseja reabrir a compra de mercadoria de número ' + item.id + '?')) {
        return;
      }

      var vm = this;

      return Servicos.Compras.ComprasMercadorias.reabrir(item.id)
        .then(function (resposta) {
          vm.atualizarLista();
        })
        .catch(function (erro) {
          if (erro && erro.mensagem) {
            vm.exibirMensagem('Erro', erro.mensagem);
          }
        });
    },

    /**
     * Atualiza a lista de compras de mercadorias.
     */
    atualizarLista: function () {
      this.$refs.lista.atualizar(true);
    },
  }
});
